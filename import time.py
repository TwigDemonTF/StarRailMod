import datetime
import os

# Set the desired Unix Time (Epoch Time) value
desired_unix_time = 1646212800  # Example Unix Time for March 2, 2022

# Convert Unix Time to datetime object
desired_datetime = datetime.datetime.utcfromtimestamp(desired_unix_time)

# Format the datetime string for tzutil command
formatted_datetime = desired_datetime.strftime('%Y-%m-%dT%H:%M:%S')

# Command to set system time using tzutil (requires elevated privileges)
os.system(f'tzutil /s "{formatted_datetime}"')

print(f"System clock set to Unix Time {desired_unix_time} ({desired_datetime})")
