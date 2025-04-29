# Define core parameters
algorithms <- c("BruteForceAlgorithm", "MVRAlgorithm", "MVRAlgorithm2")
difficulties <- c("Easy", "Medium", "Hard", "Expert")

# Define paths
raw_data_path <- "./RData/data/raw-data"
graphs_dir <- "./RData/data/graphs"
root_dir <- "./"

# 1. Outlier detection and separation (not used in graphs)
detect_outliers <- function(data) {
  groups <- split(data, interaction(data$algorithm, data$difficulty, drop = TRUE))

  processed <- lapply(groups, function(group) {
    Q1 <- quantile(group$elapsed_time_ms, 0.25, na.rm = TRUE)
    Q3 <- quantile(group$elapsed_time_ms, 0.75, na.rm = TRUE)
    IQR <- Q3 - Q1
    group$is_outlier <- group$elapsed_time_ms > (Q3 + 1.5 * IQR)
    group
  })

  do.call(rbind, processed)
}

# Data processing with entry count check
process_data <- function() {
  all_data <- list()

  for (algo in algorithms) {
    algo_dir <- file.path(raw_data_path, algo)
    files <- list.files(algo_dir, pattern = "\\.csv$", full.names = TRUE)

    for (f in files) {
      df <- read.csv(f)
      if (nrow(df) < 100) stop("File ", f, " has less than 100 entries.")

      names(df) <- trimws(names(df))
      colnames(df)[colnames(df) == "elapsed.time..ns."] <- "elapsed_time_ns"
      colnames(df)[colnames(df) == "memory.used..bytes."] <- "memory_used_bytes"

      difficulty <- gsub(".*-([A-Za-z]+).*", "\\1", basename(f))
      if(!difficulty %in% difficulties) stop("Invalid difficulty in: ", basename(f))

      df$elapsed_time_ms <- df$elapsed_time_ns / 1e6
      df$algorithm <- algo
      df$difficulty <- factor(difficulty, levels = difficulties)
      df$filename <- basename(f)  # Track filename for averaging
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

# 2. MVR Scatter Plots with averaged points
generate_mvr_scatter <- function(data) {
  mvr_data <- data[data$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2"), ]
  mvr_data$algorithm <- factor(mvr_data$algorithm,
                              levels = c("MVRAlgorithm", "MVRAlgorithm2"),
                              labels = c("MVR1", "MVR2"))

  # Function to average data
  average_data <- function(df, parts) {
    df$group <- floor((seq_len(nrow(df)) - 1) * parts / nrow(df)) + 1
    aggregate(cbind(index, elapsed_time_ms) ~ group + filename + algorithm + difficulty,
              data = df, FUN = mean)
  }

  # Process each file into 100 points
  averaged_data <- lapply(split(mvr_data, mvr_data$filename), function(file_data) {
    average_data(file_data, 100)
  })
  mvr_avg <- do.call(rbind, averaged_data)

  for (diff_level in difficulties) {
    plot_data <- mvr_avg[mvr_avg$difficulty == diff_level, ]
    if (nrow(plot_data) == 0) next

    colors <- ifelse(plot_data$algorithm == "MVR1", "blue", "red")
    fname <- file.path(graphs_dir, paste0("MVR_Comparison_", diff_level, ".pdf"))

    pdf(fname, width = 10, height = 6)
    plot(plot_data$index, plot_data$elapsed_time_ms,
         col = colors, pch = 19, cex = 0.7,
         main = paste("MVR Comparison -", diff_level),
         xlab = "Puzzle Index", ylab = "Time (ms)")

    # Add trend lines
    for (algo in c("MVR1", "MVR2")) {
      algo_data <- plot_data[plot_data$algorithm == algo, ]
      if (nrow(algo_data) > 1) {
        model <- lm(elapsed_time_ms ~ index, data = algo_data)
        abline(model, col = ifelse(algo == "MVR1", "blue", "red"), lty = 2, lwd = 2)
      }
    }

    legend("topright", legend = c("MVR1", "MVR2"), pch = 19, col = c("blue", "red"))
    dev.off()
  }
}

# 3. Bar Charts using original data (with outliers)
generate_bar_charts <- function(data_list) {
  message("Data validation:")
  message("- Original data points: ", nrow(data_list$original))
  message("- Outliers detected: ", nrow(data_list$original) - nrow(data_list$filtered))

  avg_times <- aggregate(elapsed_time_ms ~ algorithm + difficulty,
                        data = data_list$original,
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

# 4. MVR Difficulty Transition with 400 points per algorithm
generate_mvr_line <- function(data_list) {
  transition_data <- data_list$original[data_list$original$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2"), ]

  # Average each file into 400 points
  average_data <- function(df, parts) {
    df$group <- floor((seq_len(nrow(df)) - 1) * parts / nrow(df)) + 1
    aggregate(cbind(index, elapsed_time_ms) ~ group + filename + algorithm + difficulty,
              data = df, FUN = mean)
  }
  averaged <- lapply(split(transition_data, transition_data$filename), function(file_data) {
    average_data(file_data, 400)
  })
  plot_data <- do.call(rbind, averaged)
  plot_data$algorithm <- factor(plot_data$algorithm,
                               levels = c("MVRAlgorithm", "MVRAlgorithm2"),
                               labels = c("MVR1", "MVR2"))

  difficulty_max <- aggregate(index ~ difficulty, transition_data, max)
  transitions <- difficulty_max$index[-nrow(difficulty_max)]

  pdf(file.path(graphs_dir, "MVR_Difficulty_Transition.pdf"), width = 12, height = 6)
  plot(NA,
       xlim = range(plot_data$index),
       ylim = range(plot_data$elapsed_time_ms),
       main = "MVR Performance Across Difficulties",
       xlab = "Puzzle Index", ylab = "Time (ms)")

  colors <- c("blue", "red")
  for (i in 1:2) {
    algo_data <- plot_data[plot_data$algorithm == levels(plot_data$algorithm)[i], ]
    points(algo_data$index, algo_data$elapsed_time_ms, col = colors[i], pch = 19, cex = 0.5)
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

# 5. Box Plots for MVR algorithms
generate_mvr_boxplots <- function(data) {
  mvr_data <- data[data$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2"), ]
  mvr_data$algorithm <- factor(mvr_data$algorithm,
                               levels = c("MVRAlgorithm", "MVRAlgorithm2"),
                               labels = c("MVR1", "MVR2"))

  for (diff_level in difficulties) {
    plot_data <- mvr_data[mvr_data$difficulty == diff_level, ]
    if (nrow(plot_data) == 0) next

    fname <- file.path(graphs_dir, paste0("BoxPlot_", diff_level, ".pdf"))
    pdf(fname, width = 8, height = 6)
    boxplot(elapsed_time_ms ~ algorithm, data = plot_data,
            col = c("blue", "red"),
            main = paste("MVR Box Plot -", diff_level),
            xlab = "Algorithm", ylab = "Time (ms)")
    dev.off()
  }
}

# 5. Combined Report (updated)
generate_combined_report <- function(data_list) {
  pdf(file.path(root_dir, "Combined_Report.pdf"), width = 11, height = 8.5)
  par(mfrow = c(2, 2))

  # 1. Box Plots
  mvr_data <- data_list$original[data_list$original$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2"), ]
  mvr_data$algorithm <- factor(mvr_data$algorithm,
                              levels = c("MVRAlgorithm", "MVRAlgorithm2"),
                              labels = c("MVR1", "MVR2"))

  for (diff_level in difficulties) {
    plot_data <- mvr_data[mvr_data$difficulty == diff_level, ]
    if (nrow(plot_data) == 0) next

    boxplot(elapsed_time_ms ~ algorithm, data = plot_data,
            col = c("blue", "red"),
            main = paste("MVR Box Plot -", diff_level),
            xlab = "Algorithm", ylab = "Time (ms)")
  }

  # 2. Averaged Scatter Plots
  average_data <- function(df, parts) {
    df$group <- floor((seq_len(nrow(df)) - 1) * parts / nrow(df)) + 1
    aggregate(cbind(index, elapsed_time_ms) ~ group + filename + algorithm + difficulty,
              data = df, FUN = mean)
  }

   mvr_avg <- do.call(rbind, lapply(split(mvr_data, mvr_data$filename), function(x) average_data(x, 100)))

  for (diff_level in difficulties) {
    plot_data <- mvr_avg[mvr_avg$difficulty == diff_level, ]
    if (nrow(plot_data) > 0) {
      colors <- ifelse(plot_data$algorithm == "MVR1", "blue", "red")
      plot(plot_data$index, plot_data$elapsed_time_ms,
           col = colors, pch = 19, cex = 0.7,
           main = paste("Averaged Comparison -", diff_level),
           xlab = "Index", ylab = "Time (ms)")

      for (algo in c("MVR1", "MVR2")) {
        algo_data <- plot_data[plot_data$algorithm == algo, ]
        if (nrow(algo_data) > 1) {
          model <- lm(elapsed_time_ms ~ index, data = algo_data)
          abline(model, col = ifelse(algo == "MVR1", "blue", "red"), lwd = 2, lty = 2)
        }
      }
    }
  }

  # 3. Enhanced Transition Plot
  transition_avg <- do.call(rbind, lapply(split(mvr_data, mvr_data$filename), function(x) average_data(x, 400)))
  difficulty_max <- aggregate(index ~ difficulty, mvr_data, max)
  transitions <- difficulty_max$index[-nrow(difficulty_max)]

  plot(NA, xlim = range(transition_avg$index), ylim = range(transition_avg$elapsed_time_ms),
       main = "Enhanced Difficulty Transition", xlab = "Index", ylab = "Time (ms)")

  colors <- c("blue", "red")
  for (i in 1:2) {
    algo_data <- transition_avg[transition_avg$algorithm == c("MVR1", "MVR2")[i], ]
    points(algo_data$index, algo_data$elapsed_time_ms, col = colors[i], pch = 19, cex = 0.5)
    lines(algo_data$index, algo_data$elapsed_time_ms, col = colors[i], lwd = 1.5)
  }
  abline(v = transitions, lty = 2, col = "gray50")

  dev.off()
}

# 6. Full Scatter Plots for Easy Difficulty
generate_easy_full_scatter <- function(data_list) {
  easy_data <- data_list$original[data_list$original$difficulty == "Easy" &
                                  data_list$original$algorithm %in% c("MVRAlgorithm", "MVRAlgorithm2", "BruteForceAlgorithm"), ]

  easy_data$algorithm <- factor(easy_data$algorithm,
                               levels = c("MVRAlgorithm", "MVRAlgorithm2", "BruteForceAlgorithm"),
                               labels = c("MVR1", "MVR2", "BF"))

  for (algo in c("MVR1", "MVR2", "BF")) {
    plot_data <- easy_data[easy_data$algorithm == algo, ]
    if (nrow(plot_data) == 0) next

    color <- switch(algo, "MVR1" = "blue", "MVR2" = "red", "BF" = "green")
    fname <- file.path(graphs_dir, paste0(algo, "_Easy_FullScatter.pdf"))

    pdf(fname, width = 10, height = 6)
    plot(plot_data$index, plot_data$elapsed_time_ms,
         col = color, pch = 19, cex = 0.5,
         main = paste(algo, "Full Data - Easy Difficulty"),
         xlab = "Puzzle Index", ylab = "Time (ms)")

    # Add trend line
    if (nrow(plot_data) > 1) {
      abline(lm(elapsed_time_ms ~ index, plot_data),
             col = "black", lty = 2, lwd = 1.5)
    }

    legend("topright",
           legend = c("Data Points", "Trend Line"),
           col = c(color, "black"),
           pch = c(19, NA),
           lty = c(NA, 2))
    dev.off()
  }
}

# Generate all outputs
generate_mvr_scatter(data$original)  # Uses averaged points
generate_bar_charts(data)  # Uses original data
generate_mvr_line(data)     # Uses original data with averaging
generate_mvr_boxplots(data$original)
generate_combined_report(data)  # Ensure this uses original data
generate_easy_full_scatter(data)

message("Analysis complete! Check:\n- Individual graphs: ", graphs_dir, "\n- Combined report: ", file.path(root_dir, "Combined_Report.pdf"))
## 3. Bar Charts
#generate_bar_charts <- function(data_list) {
#  message("Data validation:")
#  message("- Original data points: ", nrow(data_list$original))
#  message("- Filtered data points: ", nrow(data_list$filtered))
#  message("- Outliers removed: ", nrow(data_list$original) - nrow(data_list$filtered))
#
#  avg_times <- aggregate(elapsed_time_ms ~ algorithm + difficulty,
#                        data = data_list$filtered,
#                        FUN = mean)
#
#  for (diff_level in difficulties) {
#    plot_data <- avg_times[avg_times$difficulty == diff_level, ]
#    if (nrow(plot_data) == 0) next
#
#    colors <- sapply(plot_data$algorithm, function(a) {
#      switch(a, BruteForceAlgorithm = "gray50", MVRAlgorithm = "blue", MVRAlgorithm2 = "red")
#    })
#
#    fname <- file.path(graphs_dir, paste0("BarChart_", diff_level, ".pdf"))
#    pdf(fname, width = 8, height = 6)
#
#    bp <- barplot(plot_data$elapsed_time_ms,
#            names.arg = plot_data$algorithm,
#            col = colors,
#            main = paste("Average Time -", diff_level),
#            xlab = "Algorithm", ylab = "Time (ms)",
#            ylim = c(0, max(plot_data$elapsed_time_ms) * 1.1))
#
#    text(x = bp, y = plot_data$elapsed_time_ms,
#         labels = round(plot_data$elapsed_time_ms, 1), pos = 3, cex = 0.8)
#    dev.off()
#  }
#}
#
