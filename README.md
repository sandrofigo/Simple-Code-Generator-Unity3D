# Simple Code Generator

[![tests](https://github.com/sandrofigo/Simple-Code-Generator-Unity3D/actions/workflows/tests.yml/badge.svg)](https://github.com/sandrofigo/Simple-Code-Generator-Unity3D/actions/workflows/tests.yml)

A library for generating source code from templates in Unity

---

## Installation

- Install [OpenUPM CLI](https://github.com/openupm/openupm-cli#installation)
- Run the following command in your Unity project folder

  ```
  openupm add com.sandrofigo.simplecodegenerator
  ```

## Usage

Create a `public static void` method in a class anywhere in your Unity project and decorate it with the `[CodeGenerationMethod]` attribute. This will make the method visible to the code generator. If you want to expose the method to the "Code Generation" menu add a `[MenuItem(...)]` attribute.

```csharp
public static class MyClass
{
    [CodeGenerationMethod]
    [MenuItem("Code Generation/Generate My Code")]
    public static void GenerateMyCode()
    {
        var data = new
        {
            Text = "Hello World!",
            Colors = new[] { "Red", "Green", "Blue" },
            Animal = new
            {
                Type = "fox",
                Color = "brown",
            }
        };

        CodeGenerator.GenerateFromTemplate("Scripts/Templates/MyTemplate.txt", "Scripts/Generated/MyTemplate.generated.cs", data);
    }
}
```

Additionally you need a template file in your project which the code generator can use.
Value names are case sensitive e.g. if you want to insert the value of the `data.Text` property in the data object you can do this by surrounding it with `{{  }}`. Accessing nested properties is the same as in C# e.g. `Animal.Color`.

```
// This is a sample template

// The value of the property "Text" is: {{ Text }}

// The quick {{ Animal.Color }} {{ Animal.Type }} jumps over the lazy dog

{{ for color in Colors }}

public class My{{ color }}Class
{
    // Some implementation
}

{{ end }}
```

The generated file `MyTemplate.generated.cs` will look like this:

```csharp
// This is a sample template

// The value of the property "Text" is: Hello World!

// The quick brown fox jumps over the lazy dog


public class MyRedClass
{
    // Some implementation
}


public class MyGreenClass
{
    // Some implementation
}


public class MyBlueClass
{
    // Some implementation
}

```

Code generation is automatically triggered when a `*.cs`, `*.txt` or `*.json` file has changed in your project. Additionally you can trigger the code generation manually under the "Code Generation" menu item.

## Community

Support this project with a ⭐️, report an issue or if you feel adventurous and would like to extend the functionality open a pull request.

