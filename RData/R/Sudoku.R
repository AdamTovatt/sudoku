.libPaths("./RData/R/r_libs")
options(repos = c(CRAN = "https://cloud.r-project.org"))
# ========================
# Core Parameters & Paths
# ========================
algorithms <- c("BruteForceAlgorithm", "MVRAlgorithm", "MVRAlgorithm2")
difficulties <- c("Easy", "Medium", "Hard", "Expert")

raw_data_path <- "./RData/data/raw-data"
graphs_dir <- "./RData/data/graphs"
root_dir <- "./"

# ========================
# Outlier Detection
# ========================
detect_outliers <- function(data) {
  # Split data into algorithm-difficulty groups
  groups <- split(
    data,
    interaction(data$algorithm, data$difficulty, drop = TRUE)
  )

  processed <- lapply(groups, function(group) {
    # Use R's built-in boxplot statistics
    bps <- boxplot.stats(group$elapsed_time_ms)

    # Mark outliers using vectorized operation
    group$is_outlier <- group$elapsed_time_ms %in% bps$out

    group
  })

  do.call(rbind, processed)
}

# ========================
# Data Processing
# ========================
process_data <- function() {
  all_data <- list()

  for (algo in algorithms) {
    algo_dir <- file.path(raw_data_path, algo)
    files <- list.files(algo_dir, pattern = "\\.csv$", full.names = TRUE)

    for (f in files) {
      df <- read.csv(f)
      if (nrow(df) < 100) stop("File ", f, " has less than 100 entries.")

      # Clean column names
      names(df) <- trimws(names(df))
      colnames(df) <- gsub("\\.+", "_", tolower(colnames(df)))

      # Convert ns to ms
      df$elapsed_time_ms <- df$elapsed_time_ns / 1e6

      # Extract difficulty from filename
      difficulty <- gsub(".*-([A-Za-z]+).*", "\\1", basename(f))
      if (!difficulty %in% difficulties) {
        stop("Invalid difficulty in: ", basename(f))
      }

      # Add metadata
      df$algorithm <- algo
      df$difficulty <- factor(difficulty, levels = difficulties)
      df$filename <- basename(f)

      all_data[[length(all_data) + 1]] <- df
    }
  }

  combined <- do.call(rbind, all_data)

  # Detect and remove outliers
  with_outliers <- detect_outliers(combined)

  list(
    original = combined,
    filtered = with_outliers[
      !with_outliers$is_outlier,
      -which(names(with_outliers) == "is_outlier")
    ]
  )
}

# ========================
# Outlier Reporting
# ========================
report_outlier_stats <- function(original_data, filtered_data) {
  message("Outlier Statistics:")
  message("- Original data points: ", nrow(original_data))
  message("- Filtered data points: ", nrow(filtered_data))
  message("- Outliers removed: ", nrow(original_data) - nrow(filtered_data))
}

# ========================
# Plotting Functions
# ========================
generate_mvr_scatter <- function(filtered_data) {
  mvr_data <- filtered_data[
    filtered_data$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2"),
  ]
  mvr_data$algorithm <- factor(
    mvr_data$algorithm,
    levels = c("MVRAlgorithm", "MVRAlgorithm2"),
    labels = c("MVR1", "MVR2")
  )

  # Function to average data
  average_data <- function(df, parts) {
    df$group <- floor((seq_len(nrow(df)) - 1) * parts / nrow(df)) + 1
    aggregate(
      cbind(index, elapsed_time_ms) ~ group + filename + algorithm + difficulty,
      data = df,
      FUN = mean
    )
  }

  # Process each file into 100 points
  averaged_data <- lapply(
    split(mvr_data, mvr_data$filename),
    function(file_data) {
      average_data(file_data, 100)
    }
  )
  mvr_avg <- do.call(rbind, averaged_data)

  for (diff_level in difficulties) {
    plot_data <- mvr_avg[mvr_avg$difficulty == diff_level, ]
    if (nrow(plot_data) == 0) next

    colors <- ifelse(plot_data$algorithm == "MVR1", "blue", "red")
    fname <- file.path(
      graphs_dir,
      paste0("MVR_Comparison_", diff_level, ".pdf")
    )

    pdf(fname, width = 10, height = 6)
    plot(
      plot_data$index,
      plot_data$elapsed_time_ms,
      col = colors,
      pch = 19,
      cex = 0.7,
      main = paste("MVR Comparison -", diff_level),
      xlab = "Puzzle Index",
      ylab = "Time (ms)"
    )

    # Add trend lines
    for (algo in c("MVR1", "MVR2")) {
      algo_data <- plot_data[plot_data$algorithm == algo, ]
      if (nrow(algo_data) > 1) {
        model <- lm(elapsed_time_ms ~ index, data = algo_data)
        abline(
          model,
          col = ifelse(algo == "MVR1", "blue", "red"),
          lty = 2,
          lwd = 2
        )
      }
    }

    legend(
      "topright",
      legend = c("MVR1", "MVR2"),
      pch = 19,
      col = c("blue", "red")
    )
    dev.off()
  }
}

generate_bar_charts <- function(filtered_data) {
  # Modified to work with filtered_data dataframe directly
  avg_times <- aggregate(
    elapsed_time_ms ~ algorithm + difficulty,
    data = filtered_data,
    FUN = mean
  )
  for (diff_level in difficulties) {
    plot_data <- avg_times[avg_times$difficulty == diff_level, ]
    if (nrow(plot_data) == 0) next

    colors <- sapply(plot_data$algorithm, function(a) {
      switch(
        a,
        BruteForceAlgorithm = "gray50",
        MVRAlgorithm = "blue",
        MVRAlgorithm2 = "red"
      )
    })

    fname <- file.path(graphs_dir, paste0("BarChart_", diff_level, ".pdf"))
    pdf(fname, width = 8, height = 6)

    bp <- barplot(
      plot_data$elapsed_time_ms,
      names.arg = plot_data$algorithm,
      col = colors,
      main = paste("Average Time -", diff_level),
      xlab = "Algorithm",
      ylab = "Time (ms)",
      ylim = c(0, max(plot_data$elapsed_time_ms) * 1.1)
    )

    text(
      x = bp,
      y = plot_data$elapsed_time_ms,
      labels = round(plot_data$elapsed_time_ms, 1),
      pos = 3,
      cex = 0.8
    )
    dev.off()
  }
}


generate_mvr_boxplots <- function(filtered_data) {
  mvr_data <- filtered_data[
    filtered_data$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2"),
  ]
  mvr_data$algorithm <- factor(
    mvr_data$algorithm,
    levels = c("MVRAlgorithm", "MVRAlgorithm2"),
    labels = c("MVR1", "MVR2")
  )

  for (diff_level in difficulties) {
    plot_data <- mvr_data[mvr_data$difficulty == diff_level, ]
    if (nrow(plot_data) == 0) next

    fname <- file.path(graphs_dir, paste0("BoxPlot_", diff_level, ".pdf"))
    pdf(fname, width = 8, height = 6)
    boxplot(
      elapsed_time_ms ~ algorithm,
      data = plot_data,
      col = c("blue", "red"),
      main = paste("MVR Box Plot -", diff_level),
      xlab = "Algorithm",
      ylab = "Time (ms)"
    )
    dev.off()
  }
}

generate_easy_full_scatter <- function(filtered_data) {
  easy_data <- filtered_data[
    filtered_data$difficulty == "Easy" &
      filtered_data$algorithm %in%
        c("MVRAlgorithm", "MVRAlgorithm2", "BruteForceAlgorithm"),
  ]

  easy_data$algorithm <- factor(
    easy_data$algorithm,
    levels = c("MVRAlgorithm", "MVRAlgorithm2", "BruteForceAlgorithm"),
    labels = c("MVR1", "MVR2", "BF")
  )

  for (algo in c("MVR1", "MVR2", "BF")) {
    plot_data <- easy_data[easy_data$algorithm == algo, ]
    if (nrow(plot_data) == 0) next

    color <- switch(algo, "MVR1" = "blue", "MVR2" = "red", "BF" = "green")
    fname <- file.path(graphs_dir, paste0(algo, "_Easy_FullScatter.pdf"))

    pdf(fname, width = 10, height = 6)
    plot(
      plot_data$index,
      plot_data$elapsed_time_ms,
      col = color,
      pch = 19,
      cex = 0.5,
      main = paste(algo, "Full Data - Easy Difficulty"),
      xlab = "Puzzle Index",
      ylab = "Time (ms)"
    )

    # Add trend line
    if (nrow(plot_data) > 1) {
      abline(
        lm(elapsed_time_ms ~ index, plot_data),
        col = "black",
        lty = 2,
        lwd = 1.5
      )
    }

    legend(
      "topright",
      legend = c("Data Points", "Trend Line"),
      col = c(color, "black"),
      pch = c(19, NA),
      lty = c(NA, 2)
    )
    dev.off()
  }
}

# ========================
# Combined Report
# ========================
generate_combined_report <- function() {
  if (!require(pdftools)) install.packages("pdftools")
  library(pdftools)

  output_file <- file.path(root_dir, "Combined_Report.pdf")
  plot_files <- list.files(graphs_dir, pattern = "\\.pdf$", full.names = TRUE)

  if (length(plot_files) > 0) {
    pdf_combine(plot_files, output = output_file)
    message("Combined report created from existing plots")
  } else {
    warning("No plot files found to combine")
  }
}

generate_bell_curve <- function(filtered_data) {
  # Create histogram data
  h <- hist(filtered_data$elapsed_time_ms, plot = FALSE)

  # Calculate normal curve parameters
  x <- filtered_data$elapsed_time_ms
  xfit <- seq(min(x), max(x), length = 100)
  yfit <- dnorm(xfit, mean = mean(x), sd = sd(x))

  # Scale normal curve to histogram
  yfit <- yfit * diff(h$mids[1:2]) * length(x)

  # Create plot
  pdf(file.path(graphs_dir, "Bell_Curve.pdf"), width = 8, height = 6)
  hist(
    x,
    col = "lightblue",
    main = "Execution Time Distribution with Bell Curve",
    xlab = "Time (ms)",
    ylab = "Frequency",
    probability = FALSE
  ) # Keep raw counts

  lines(xfit, yfit, col = "red", lwd = 2)
  legend(
    "topright",
    legend = c("Data", "Normal Curve"),
    col = c("lightblue", "red"),
    lty = c(1, 1),
    lwd = c(10, 2)
  )
  dev.off()
}

# ========================
# Execution Pipeline
# ========================
# Process data and get filtered dataset
processed_data <- process_data()
filtered_data <- processed_data$filtered

# Report statistics
report_outlier_stats(processed_data$original, filtered_data)

# Generate individual plots
generate_mvr_scatter(filtered_data)
generate_bar_charts(filtered_data)
generate_mvr_boxplots(filtered_data)
generate_easy_full_scatter(filtered_data)
generate_bell_curve(filtered_data)

# Combine existing plots into report
generate_combined_report()

message(
  "Analysis complete! Check:\n- Individual graphs: ",
  graphs_dir,
  "\n- Combined report: ",
  file.path(root_dir, "Combined_Report.pdf")
)
