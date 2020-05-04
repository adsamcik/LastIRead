**NO LONGER IN DEVELOPMENT**

Application was rewritten to Flutter and will be further developed in different repo [https://github.com/adsamcik/MarkMyProgress](https://github.com/adsamcik/MarkMyProgress). Reason for this change was primarily that WPF UI is way too inflexible. Migrating data from this app to the new one requires exporting to json and manually changing all the variable names to camelCase.

# LastIRead

Free open-source application for tracking your reading progress.

## Technical

Supports export and import from CSV and JSON.
Data is internally stored using LiteDB into a `reading_data.db` file in the same directory as launch file.
Written in C# using .NET Core 3.1 with WPF (=> Windows only at least for now).
