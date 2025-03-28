Calculator Application
This is a C# WPF-based calculator application that supports both standard arithmetic operations and a programmer mode with multiple numeral systems. The project is designed with modularity in mind, separating the UI orchestration from the logic handling, memory operations, and numerical input parsing. Below is an overview of its key features and implementation details.

Key Features
Basic Arithmetic Operations:
Perform addition, subtraction, multiplication, and division. The calculator processes operations sequentially (without operator precedence), supporting cascading operations by evaluating intermediate results.

Programmer Mode:
In addition to the standard decimal mode, the calculator supports binary, octal, and hexadecimal numeral systems. The UI adapts by enabling/disabling specific buttons (e.g., A–F are only active in hexadecimal mode).

Memory Operations:
Save, recall, add to, and subtract from stored memory values. The application maintains a memory stack that can be navigated via a selection dialog.

Digit Grouping:
When enabled, numeric values are formatted with appropriate digit grouping (e.g., thousands separators) based on the current locale.

Clipboard Support:
Provides functionalities for copying, cutting, and pasting numbers. Pasted values are validated and converted according to the current numeral base.

Configuration Persistence:
User preferences such as digit grouping, calculator mode (standard or programmer), and the current numeral base are stored in simple text files. These settings are loaded at startup, ensuring that the user’s configuration persists across sessions.

Implementation Details
UI and Code-Behind:
The UI is defined in XAML (not included here) with the logic handled in the code-behind file MainWindow.xaml.cs. This file initializes the main window, binds UI events to handlers, and orchestrates the various modules.

Handler Modules:
The project is organized into several dedicated classes:

NumericButtonsHandler:
Manages numeric input, base conversions, and formatting. It handles adding digits to the display and formatting numbers based on the active grouping settings.

OperationsHandler:
Implements arithmetic operations, including parsing input values, performing calculations, and formatting the output based on the current numeral base.

MemoryAndMiscHandler:
Provides auxiliary functionalities such as memory storage and retrieval, clipboard operations, and other miscellaneous actions (e.g., an About dialog).

Parsing and Data Processing:
While the project does not implement a full-fledged expression parser, it processes user input sequentially. Each digit or operator is appended to the display, and the current value is parsed using double.Parse (for decimal) or Convert.ToInt32 (for other bases) as needed. This simple approach is sufficient for the intended functionality.

Flags and State Management:
Several flags and state variables are used throughout the application:

Exception: Indicates if an error occurred during an operation (e.g., division by zero or invalid input).

OperationFlag: Signals when an operation has been initiated, which helps in determining when to reset the display or append to the current value.

MemoryFlag: Tracks whether there is a stored memory value available.

CurrentBase: Represents the active numeral base (10, 2, 8, or 16), affecting both input parsing and output formatting.

Checked: Reflects whether digit grouping is enabled.

IsProgrammerMode: Determines whether the application is in Programmer mode (allowing non-decimal numeral operations) or Standard mode.
