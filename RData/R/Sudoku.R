# =================================
# Library Path & Required Packages
# =================================
.libPaths("./RData/R/r_libs")
options(repos = c(CRAN = "https://cloud.r-project.org"))

if (!require(dplyr)) install.packages("dplyr")
library(dplyr)

if (!require(dunn.test)) install.packages("dunn.test")
library(dunn.test)

if (!require(pdftools)) install.packages("pdftools")
library(pdftools)

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

# saves outlier counts per difficulty per algorithm.
save_outlier_counts <- function(with_outliers) {
  outlier_counts <- aggregate(
    is_outlier ~ algorithm + difficulty,
    data = with_outliers,
    FUN = sum
  )
  write.csv(
    outlier_counts,
    file.path(graphs_dir, "outlier_counts.csv"),
    row.names = FALSE
  )
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
    original = with_outliers,
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
# Modified Plotting Function
# ========================
generate_bar_charts <- function(
  data_source,
  use_mean = TRUE,
  pdf_name = "BarChart_AllDifficulties.pdf",
  chart_title = "Average Time by Algorithm and Difficulty"
) {
  # Aggregate based on mean/median choice
  agg_times <- aggregate(
    elapsed_time_ms ~ algorithm + difficulty,
    data = data_source,
    FUN = if (use_mean) mean else median
  )

  # Reshape to wide format
  plot_data <- reshape(
    agg_times,
    timevar = "difficulty",
    idvar = "algorithm",
    direction = "wide"
  )

  # Replace NAs with 0 in numeric columns
  plot_data[is.na(plot_data)] <- 0

  # Prepare colors for algorithms
  algorithm_colors <- c(
    BruteForceAlgorithm = "gray50",
    MVRAlgorithm = "blue",
    MVRAlgorithm2 = "red",
    PreprocessAlgorithm = "green"
  )

  # Generate bar chart
  suppressWarnings({
    fname <- file.path(graphs_dir, pdf_name)
    pdf(fname, width = 10, height = 6)

    bar_positions <- barplot(
      as.matrix(plot_data[, -1]),
      beside = TRUE,
      col = algorithm_colors[plot_data$algorithm],
      main = chart_title,
      xlab = "Difficulty",
      ylab = if (use_mean) "Time (ms) - Mean" else "Time (ms) - Median",
      names.arg = difficulties,
      legend.text = plot_data$algorithm,
      args.legend = list(x = "topright", bty = "n"),
      ylim = c(0, max(as.matrix(plot_data[, -1])) * 1.4)
    )

    # Add labels with specified decimal places
    text(
      x = as.vector(bar_positions),
      y = as.vector(as.matrix(plot_data[, -1])),
      labels = round(as.vector(as.matrix(plot_data[, -1])), 2),
      pos = 3,
      cex = 0.8
    )

    dev.off()
  })
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

  suppressWarnings({
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
      legend = c("MVR1", "MVR2"),
      fill = c("blue", "red"),
      bty = "n"
    )
    dev.off()
  })
}

perform_pairwise_tests <- function(filtered_data) {
  difficulties <- unique(filtered_data$difficulty)
  algorithms <- unique(filtered_data$algorithm)

  sink(file.path(graphs_dir, "pairwise_tests.txt"))
  for (diff in difficulties) {
    cat("\n===== Difficulty:", diff, "=====\n")
    subset <- filtered_data[filtered_data$difficulty == diff, ]

    # Non-parametric (Mann-Whitney)
    cat("\n-- Non-parametric (Mann-Whitney) --\n")
    pairwise.wilcox.test(
      subset$elapsed_time_ms,
      subset$algorithm,
      p.adjust.method = "bonferroni"
    ) %>%
      print()

    # Non-parametric (Dunn's Test)
    cat("\n-- Non-parametric (Dunn's Test) --\n")
    dunn_results <- dunn.test::dunn.test(
      x = subset$elapsed_time_ms,
      g = subset$algorithm,
      method = "bonferroni"
    )
    print(dunn_results)
  }
  sink()
}

perform_omnibus_tests <- function(filtered_data) {
  sink(file.path(graphs_dir, "omnibus_tests.txt"))
  for (diff in difficulties) {
    subset <- filtered_data[filtered_data$difficulty == diff, ]

    cat("\n===== Difficulty:", diff, "=====\n")

    # Parametric (ANOVA)
    cat("\n-- ANOVA --\n")
    anova_result <- aov(elapsed_time_ms ~ algorithm, data = subset)
    print(summary(anova_result))

    # Non-parametric (Kruskal-Wallis)
    cat("\n-- Kruskal-Wallis --\n")
    kw_result <- kruskal.test(elapsed_time_ms ~ algorithm, data = subset)
    print(kw_result)
  }
  sink()
}

check_normality <- function(filtered_data) {
  suppressWarnings({
    pdf(file.path(graphs_dir, "qq_plots.pdf"), width = 10, height = 6)
    par(mfrow = c(4, 4)) # 4 algorithms x 4 difficulties

    for (algo in algorithms) {
      for (diff in difficulties) {
        subset <- filtered_data[
          filtered_data$algorithm == algo & filtered_data$difficulty == diff,
          "elapsed_time_ms"
        ]

        qqnorm(subset, main = paste(algo, "-", diff))
        qqline(subset, col = "red")
      }
    }
    dev.off()
  })
}

generate_all_data_histogram <- function(
  Data,
  algorithm,
  difficulty,
  max_x = NULL
) {
  # Filter data for specific algorithm and difficulty
  subset_data <- Data[
    Data$algorithm == algorithm & Data$difficulty == difficulty,
    "elapsed_time_ms"
  ]

  # Check if there's data to plot
  if (length(subset_data) == 0) {
    stop("No data found for ", algorithm, " - ", difficulty)
  }

  if (!is.null(max_x)) {
    subset_data <- subset_data[subset_data <= max_x]
    if (length(subset_data) == 0) {
      stop("No data remaining after cutoff for ", algorithm, " - ", difficulty)
    }
  }
  # Create filename with algorithm and difficulty
  clean_algo <- gsub(" ", "", algorithm)
  clean_diff <- gsub(" ", "", difficulty)
  fname <- file.path(
    graphs_dir,
    paste0("histogram_", clean_algo, "_", clean_diff, ".pdf")
  )

  # Create histogram with 100 bins
  suppressWarnings({
    pdf(fname, width = 10, height = 6)
    hist(
      subset_data,
      breaks = 1000,
      main = paste("Time Distribution:", algorithm, "-", difficulty),
      xlab = "Elapsed Time (ms)",
      ylab = "Frequency",
      col = "skyblue",
      border = "white",
    )
    dev.off()
  })

  message("Created histogram: ", fname)
}

generate_single_qq_plot <- function(filtered_data, algorithm, difficulty) {
  subset <- filtered_data[
    filtered_data$algorithm == algorithm &
      filtered_data$difficulty == difficulty,
    "elapsed_time_ms"
  ]
  
  suppressWarnings({
    pdf(
      file.path(
        graphs_dir,
        paste0("qq_plot_", algorithm, "_", difficulty, ".pdf")
      ),
      width = 10,
      height = 6
    )
    qqnorm(subset, main = paste(algorithm, "-", difficulty))
    qqline(subset, col = "red")
    dev.off()
  })
}

# ========================
# Combined Report
# ========================
generate_combined_report <- function() {

  output_file <- file.path(root_dir, "Combined_Report.pdf")
  plot_files <- list.files(graphs_dir, pattern = "\\.pdf$", full.names = TRUE)

  suppressWarnings({
    if (length(plot_files) > 0) {
      pdf_combine(plot_files, output = output_file)
      message("Combined report created from existing plots")
    } else {
      warning("No plot files found to combine")
    }
  })
}

# ========================
# Execution Pipeline
# ========================
# Process data and get filtered dataset
processed_data <- process_data()
save_outlier_counts(processed_data$original)
filtered_data <- processed_data$filtered

# Report statistics
generate_single_qq_plot(
  filtered_data,
  "BruteForceAlgorithm",
  "Expert"
)


# Generate individual plots 

# ================
# These are the graphs that weren't used in
# the final raport as they are useless.
# If you still wish to see the results here it is.
# generate_mvr_boxplots(filtered_data)

#generate_all_data_histogram(
#  processed_data$original,
#  "BruteForceAlgorithm",
#  "Expert",
#  20
#)

#generate_bar_charts(filtered_data)
#
#generate_bar_charts(
#  data_source = processed_data$original,
#  use_mean = TRUE,
#  pdf_name = "BarChart_OriginalData_Mean.pdf",
#  chart_title = "Original Data - Mean Times by Algorithm and Difficulty"
#)

#generate_bar_charts(
#  data_source = processed_data$original,
#  use_mean = FALSE,
#  pdf_name = "BarChart_OriginalData_Median.pdf",
#  chart_title = "Original Data - Median Times by Algorithm and Difficulty"
#)

# check_normality(filtered_data)
# =================

generate_bar_charts(
  data_source = filtered_data,
  use_mean = FALSE,
  pdf_name = "BarChart_FilteredData_Median.pdf",
  chart_title = "Filtered Data - Median Times by Algorithm and Difficulty"
)

median_table <- aggregate(
  elapsed_time_ms ~ algorithm + difficulty,
  data = filtered_data,
  FUN = median
)

# Combine existing plots into report
generate_combined_report()

message(
  "Analysis complete! Check:\n- Individual graphs and test results: ",
  graphs_dir,
  "\n- Combined report of pdf documents: ",
  file.path(root_dir, "Combined_Report.pdf")
)
print(median_table)
