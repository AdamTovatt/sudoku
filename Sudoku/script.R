# Load required libraries
library(ggplot2)
library(tidyr)
library(dplyr)
library(gridExtra)

# Read the CSV file
data <- read.csv("improveMVR2.csv", stringsAsFactors = FALSE)

# Function to extract bit positions for '1' in binary string
extract_bit_positions <- function(binary_str) {
  # Ensure binary_str is a character string
  binary_str <- as.character(binary_str)
  
  # Convert binary string to a list of positions where '1' occurs
  positions <- which(strsplit(binary_str, NULL)[[1]] == "1")
  return(positions)
}

# Apply the function to the 'avaliable_binary' column
data$bit_positions <- sapply(data$avaliable_binary, extract_bit_positions)

# Unnest the bit positions to have one bit position per row
data_long <- data %>%
  unnest(bit_positions) %>%
  select(possibleNumber, bit_positions)

# Function to plot value counts for a given column
plot_value_counts <- function(column_name) {
  value_counts <- as.data.frame(table(data[[column_name]]))
  colnames(value_counts) <- c("Value", "Count")
  
  ggplot(value_counts, aes(x = Value, y = Count)) +
    geom_bar(stat = "identity", fill = "steelblue") +
    theme_minimal() +
    labs(title = paste("Frequency of values in", column_name),
         x = column_name, y = "Count") +
    theme(axis.text.x = element_text(angle = 45, hjust = 1))
}

# Create individual plots
plot1 <- plot_value_counts("possibleNumber")
plot2 <- ggplot(data_long, aes(x = factor(bit_positions))) +
  geom_bar(stat = "count", fill = "steelblue") +
  theme_minimal() +
  labs(title = "Frequency of Bit Positions", x = "Bit Position", y = "Count") +
  theme(axis.text.x = element_text(angle = 45, hjust = 1))
plot3 <- plot_value_counts("avaliable_count")
plot4 <- plot_value_counts("minimum_options")

# Arrange and save all plots to a single image file
png("value_frequencies_combined.png", width = 1200, height = 1000)
grid.arrange(plot1, plot2, plot3, plot4, ncol = 2)
dev.off()
