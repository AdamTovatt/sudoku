.libPaths("./RData/R/r_libs")
options(repos = c(CRAN = "https://cloud.r-project.org"))
# ========================
# Core Parameters & Paths
# ========================
algorithms <- c(
  "BruteForceAlgorithm",
  "MVRAlgorithm",
  "MVRAlgorithm2",
  "PreprocessAlgorithm"
)
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
generate_bar_charts <- function(filtered_data) {
  # Modified to work with filtered_data dataframe directly
  avg_times <- aggregate(
    elapsed_time_ms ~ algorithm + difficulty,
    data = filtered_data,
    FUN = mean
  )

  # Prepare colors for algorithms
  algorithm_colors <- c(
    BruteForceAlgorithm = "gray50",
    MVRAlgorithm = "blue",
    MVRAlgorithm2 = "red",
    PreprocessAlgorithm = "green"
  )

  # Prepare bar chart data
  difficulties <- unique(avg_times$difficulty)
  plot_data <- reshape(
    avg_times,
    timevar = "difficulty",
    idvar = "algorithm",
    direction = "wide"
  )
  plot_data[is.na(plot_data)] <- 0

  # Generate bar chart
  fname <- file.path(graphs_dir, "BarChart_AllDifficulties.pdf")
  pdf(fname, width = 10, height = 6)

  bar_positions <- barplot(
    as.matrix(plot_data[, -1]),
    beside = TRUE,
    col = algorithm_colors[plot_data$algorithm],
    main = "Average Time by Algorithm and Difficulty",
    xlab = "Difficulty",
    ylab = "Time (ms)",
    names.arg = difficulties,
    legend.text = plot_data$algorithm,
    args.legend = list(x = "topright", bty = "n")
  )

  # Add labels with specified decimal places
  text(
    x = bar_positions,
    y = as.matrix(plot_data[, -1]),
    labels = round(as.matrix(plot_data[, -1]), 3),
    pos = 3,
    cex = 0.8
  )

  dev.off()
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

  fname <- file.path(graphs_dir, "BoxPlot_AllDifficulties.pdf")
  pdf(fname, width = 12, height = 6)
  boxplot(
    elapsed_time_ms ~ interaction(algorithm, difficulty),
    data = mvr_data,
    col = c("blue", "red"),
    main = "MVR Box Plot - All Difficulties",
    xlab = "Difficulty",
    ylab = "Time (ms)",
    las = 1,
    xaxt = "n" # Suppress default x-axis labels
  )

  # Add custom x-axis labels
  difficulty_positions <- seq(
    1.5,
    by = 2,
    length.out = length(levels(mvr_data$difficulty))
  )
  axis(
    side = 1,
    at = difficulty_positions,
    labels = levels(mvr_data$difficulty),
    tick = FALSE,
    line = 0
  )

  # Add legend
  legend(
    "topright",
    legend = c("MVR1 (Blue)", "MVR2 (Red)"),
    fill = c("blue", "red"),
    bty = "n"
  )
  dev.off()
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

# ========================
# Execution Pipeline
# ========================
# Process data and get filtered dataset
processed_data <- process_data()
filtered_data <- processed_data$filtered

# Report statistics
report_outlier_stats(processed_data$original, filtered_data)

# Generate individual plots
generate_bar_charts(filtered_data)
generate_mvr_boxplots(filtered_data)

# Combine existing plots into report
generate_combined_report()

message(
  "Analysis complete! Check:\n- Individual graphs: ",
  graphs_dir,
  "\n- Combined report: ",
  file.path(root_dir, "Combined_Report.pdf")
)
