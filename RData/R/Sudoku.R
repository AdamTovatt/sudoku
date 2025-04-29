# Define core parameters
algorithms <- c("BruteForceAlgorithm", "MVRAlgorithm", "MVRAlgorithm2")
difficulties <- c("Easy", "Medium", "Hard", "Expert")

# Define paths
raw_data_path <- "./RData/data/raw-data"
graphs_dir <- "./RData/data/graphs"
root_dir <- "./"

# 1. Outlier detection and separation
detect_outliers <- function(data) {
  groups <- split(data, interaction(data$algorithm, data$difficulty, drop = TRUE))

  processed <- lapply(groups, function(group) {
    Q1 <- quantile(group$elapsed_time_ms, 0.25, na.rm = TRUE)
    Q3 <- quantile(group$elapsed_time_ms, 0.75, na.rm = TRUE)
    IQR <- Q3 - Q1
    # Only check for upper outliers
    group$is_outlier <- group$elapsed_time_ms > (Q3 + 1.5 * IQR)
    group
  })

  filtered <- do.call(rbind, processed)
  filtered[!filtered$is_outlier, ]
}

# Data processing
process_data <- function() {
  all_data <- list()

  for (algo in algorithms) {
    algo_dir <- file.path(raw_data_path, algo)
    files <- list.files(algo_dir, pattern = "\\.csv$", full.names = TRUE)

    for (f in files) {
      df <- read.csv(f)
      names(df) <- trimws(names(df))
      colnames(df)[colnames(df) == "elapsed.time..ns."] <- "elapsed_time_ns"
      colnames(df)[colnames(df) == "memory.used..bytes."] <- "memory_used_bytes"

      difficulty <- gsub(".*-([A-Za-z]+).*", "\\1", basename(f))
      if(!difficulty %in% difficulties) stop("Invalid difficulty in: ", basename(f))

      df$elapsed_time_ms <- df$elapsed_time_ns / 1e6
      df$algorithm <- algo
      df$difficulty <- factor(difficulty, levels = difficulties)
      all_data[[length(all_data) + 1]] <- df
    }
  }

  combined <- do.call(rbind, all_data)
  list(
    original = combined,
    filtered = detect_outliers(combined)
  )
}

# Process data
data <- process_data()

# 2. MVR Scatter Plots
generate_mvr_scatter <- function(data) {
  mvr_data <- data[data$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2"), ]
  mvr_data$algorithm <- factor(mvr_data$algorithm,
                              levels = c("MVRAlgorithm", "MVRAlgorithm2"),
                              labels = c("MVR1", "MVR2"))

  for (diff_level in difficulties) {
    plot_data <- mvr_data[mvr_data$difficulty == diff_level, ]
    if (nrow(plot_data) == 0) next

    colors <- ifelse(plot_data$algorithm == "MVR1", "blue", "red")
    fname <- file.path(graphs_dir, paste0("MVR_Comparison_", diff_level, ".pdf"))

    pdf(fname, width = 10, height = 6)
    plot(plot_data$index, plot_data$elapsed_time_ms,
         col = colors, pch = 19, cex = 0.7,
         main = paste("MVR Comparison -", diff_level),
         xlab = "Puzzle Index", ylab = "Time (ms)")

    mvr1 <- plot_data[plot_data$algorithm == "MVR1", ]
    if (nrow(mvr1) > 1) abline(lm(elapsed_time_ms ~ index, mvr1), col = "blue", lty = 2, lwd = 2)
    mvr2 <- plot_data[plot_data$algorithm == "MVR2", ]
    if (nrow(mvr2) > 1) abline(lm(elapsed_time_ms ~ index, mvr2), col = "red", lty = 2, lwd = 2)

    legend("topright", legend = c("MVR1", "MVR2"), pch = 19, col = c("blue", "red"))
    dev.off()
  }
}

# 3. Bar Charts
generate_bar_charts <- function(data_list) {
  message("Data validation:")
  message("- Original data points: ", nrow(data_list$original))
  message("- Filtered data points: ", nrow(data_list$filtered))
  message("- Outliers removed: ", nrow(data_list$original) - nrow(data_list$filtered))

  avg_times <- aggregate(elapsed_time_ms ~ algorithm + difficulty,
                        data = data_list$filtered,
                        FUN = mean)

  for (diff_level in difficulties) {
    plot_data <- avg_times[avg_times$difficulty == diff_level, ]
    if (nrow(plot_data) == 0) next

    colors <- sapply(plot_data$algorithm, function(a) {
      switch(a, BruteForceAlgorithm = "gray50", MVRAlgorithm = "blue", MVRAlgorithm2 = "red")
    })

    fname <- file.path(graphs_dir, paste0("BarChart_", diff_level, ".pdf"))
    pdf(fname, width = 8, height = 6)

    bp <- barplot(plot_data$elapsed_time_ms,
            names.arg = plot_data$algorithm,
            col = colors,
            main = paste("Average Time -", diff_level),
            xlab = "Algorithm", ylab = "Time (ms)",
            ylim = c(0, max(plot_data$elapsed_time_ms) * 1.1))

    text(x = bp, y = plot_data$elapsed_time_ms,
         labels = round(plot_data$elapsed_time_ms, 1), pos = 3, cex = 0.8)
    dev.off()
  }
}

# 4. MVR Difficulty Transition
generate_mvr_line <- function(data_list) {
  transition_data <- data_list$original[data_list$original$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2"), ]
  plot_data <- data_list$filtered[data_list$filtered$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2"), ]

  difficulty_max <- aggregate(index ~ difficulty, transition_data, max)
  transitions <- difficulty_max$index[-nrow(difficulty_max)]

  plot_data <- plot_data[order(plot_data$difficulty, plot_data$index), ]
  plot_data$algorithm <- factor(plot_data$algorithm,
                               levels = c("MVRAlgorithm", "MVRAlgorithm2"),
                               labels = c("MVR1", "MVR2"))

  pdf(file.path(graphs_dir, "MVR_Difficulty_Transition.pdf"), width = 12, height = 6)
  plot(NA,
       xlim = range(plot_data$index),
       ylim = range(plot_data$elapsed_time_ms),
       main = "MVR Performance Across Difficulties",
       xlab = "Puzzle Index", ylab = "Time (ms)")

  colors <- c("blue", "red")
  for (i in 1:2) {
    algo_data <- plot_data[plot_data$algorithm == levels(plot_data$algorithm)[i], ]
    lines(algo_data$index, algo_data$elapsed_time_ms, col = colors[i], lwd = 1.5)
  }

  abline(v = transitions, lty = 2, col = "gray50")
  text(x = transitions - diff(c(0, transitions))/2,
       y = max(plot_data$elapsed_time_ms) * 0.95,
       labels = head(levels(plot_data$difficulty), -1))

  legend("topright", legend = levels(plot_data$algorithm),
         col = colors, lty = 1, lwd = 2)
  dev.off()
}

# 5. Combined Report
generate_combined_report <- function(data_list) {
  pdf(file.path(root_dir, "Combined_Report.pdf"), width = 11, height = 8.5)

  # Scatter Plots
  mvr_data <- data_list$filtered[data_list$filtered$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2"), ]
  mvr_data$algorithm <- factor(mvr_data$algorithm,
                              levels = c("MVRAlgorithm", "MVRAlgorithm2"),
                              labels = c("MVR1", "MVR2"))

  for (diff_level in difficulties) {
    plot_data <- mvr_data[mvr_data$difficulty == diff_level, ]
    if (nrow(plot_data) == 0) next

    plot(plot_data$index, plot_data$elapsed_time_ms,
         col = ifelse(plot_data$algorithm == "MVR1", "blue", "red"),
         pch = 19, cex = 0.7,
         main = paste("MVR Comparison -", diff_level),
         xlab = "Index", ylab = "Time (ms)")

    mvr1 <- plot_data[plot_data$algorithm == "MVR1", ]
    if (nrow(mvr1) > 1) abline(lm(elapsed_time_ms ~ index, mvr1), col = "blue", lwd = 2, lty = 2)
    mvr2 <- plot_data[plot_data$algorithm == "MVR2", ]
    if (nrow(mvr2) > 1) abline(lm(elapsed_time_ms ~ index, mvr2), col = "red", lwd = 2, lty = 2)
  }

  # Bar Charts
  avg_times <- aggregate(elapsed_time_ms ~ algorithm + difficulty, data_list$filtered, mean)
  for (diff_level in difficulties) {
    plot_data <- avg_times[avg_times$difficulty == diff_level, ]
    if (nrow(plot_data) == 0) next

    bp <- barplot(plot_data$elapsed_time_ms,
            names.arg = plot_data$algorithm,
            col = sapply(plot_data$algorithm, function(a) {
              switch(a, BruteForceAlgorithm = "gray50", MVRAlgorithm = "blue", MVRAlgorithm2 = "red")
            }),
            main = paste("Average Time -", diff_level),
            ylab = "Time (ms)", ylim = c(0, max(plot_data$elapsed_time_ms) * 1.1))
    text(bp, plot_data$elapsed_time_ms, round(plot_data$elapsed_time_ms, 1), pos = 3)
  }

  # Transition Plot
  plot_data <- data_list$filtered[data_list$filtered$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2"), ]
  transition_data <- data_list$original[data_list$original$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2"), ]

  difficulty_max <- aggregate(index ~ difficulty, transition_data, max)
  transitions <- difficulty_max$index[-nrow(difficulty_max)]

  plot(NA, xlim = range(plot_data$index), ylim = range(plot_data$elapsed_time_ms),
       main = "MVR Difficulty Transition", xlab = "Index", ylab = "Time (ms)")

  colors <- c("blue", "red")
  for (i in 1:2) {
    algo_data <- plot_data[plot_data$algorithm == c("MVRAlgorithm", "MVRAlgorithm2")[i], ]
    lines(algo_data$index, algo_data$elapsed_time_ms, col = colors[i], lwd = 1.5)
  }
  abline(v = transitions, lty = 2, col = "gray50")

  dev.off()
}

# Generate all outputs
generate_mvr_scatter(data$filtered)
generate_bar_charts(data)  # Pass full data list
generate_mvr_line(data)
generate_combined_report(data)  # Include combined report

message("Analysis complete! Check:\n- Individual graphs: ", graphs_dir, "\n- Combined report: ", file.path(root_dir, "Combined_Report.pdf"))
