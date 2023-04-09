# QrcodeGenerate

QrcodeGenerate is a C# Blazor WebAssembly project that generates QR codes. The user can generate a QR code in one of two ways: by entering a URL or by filling out fields for name, phone number, and work information. The generated QR code can be customized with color and icons. The project also includes a button that allows the user to add the generated contact card to their contacts as an extension.vcf file. Additionally, the project includes a library for generating QR codes that can be used in other projects.

## Getting Started

To use this project, follow these steps:

1. Clone the repository to your local machine.
2. Open the project in Visual Studio.
3. Build the solution to ensure all dependencies are installed.
4. Run the project to start the application.

## Usage

Once the project is running, follow these steps to generate a QR code:

### Generate QR Code from URL

1. Click on the "URL" tab.
2. Enter the desired URL in the input field.
3. Customize the QR code with color and icons if desired.
4. Click the "Generate QR Code" button.

### Generate QR Code from Fields

1. Click on the "Fields" tab.
2. Enter the name, phone number, and work information into the form.
3. Customize the QR code with color and icons if desired.
4. Click the "Generate QR Code" button.

### Add Contact to Contacts

1. Click on the "Add to Contacts" button.
2. The generated contact card will be saved as an extension.vcf file.

## Library Usage

To use the QR code library in another project, follow these steps:

1. Add a reference to the `QRCodeLibrary` project in your solution.
2. Import the `QRCodeLibrary` namespace in your code file.
3. Use the `QRCodeGenerator` class to generate a QR code with the desired information.


