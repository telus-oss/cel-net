# Common Expression Language
The Common Expression Language (CEL) implements common semantics for expression evaluation, enabling different applications to more easily interoperate.

This project is a native C# imlementation of the CEL Spec available [Here](https://github.com/google/cel-spec)

# Type declarations
The Common Expression Language (CEL) is a simple expression language built on top of protocol buffer types.  If you want to reference and use your own classes, they must be defined as a protocol buffer message and the descriptor registered in the CEL startup.  

Suppose a type defined as follows (using protocol buffer syntax):

```proto
package mypackage;

message Account {
  string account_number = 1;
  string display_name = 2;
  double overdraftLimit = 3;
  double balance = 4;  
  ...
}

message Transaction {
  string transaction_number = 1;
  double withdrawal = 2;
}
```

# Environment Setup
You will initialize the CEL environment with the file descriptors for your application.  These are generated using the [Google.Protobuf](https://www.nuget.org/packages/Google.Protobuf/) library.  You can use [Grpc.Tools](https://www.nuget.org/packages/Grpc.Tools) to compile them automatically in your application.  

The default namespace is optional.  It is used to simplify some CEL expressions when creating or casting types.

``` csharp  
   // build a list of the file descriptors in your application.  
   var fileDescriptors = new[]
   {
       AccountReflection.Descriptor
   };
   var typeRegistry = Google.Protobuf.Reflection.TypeRegistry.FromFiles(fileDescriptors);

   // the default namespace is optional.
   var defaultNamespace = "mypackage";

   var celEnvironment = new CelEnvironment(fileDescriptors, defaultNamespace);   
```


# CEL Expression Execution

You define your own CEL Expression. The example below will return a boolean.

``` csharp
    // An expression compatible with CEL-spec
    var celExpression = "account.balance >= transaction.withdrawal || (account.overdraftProtection && account.overdraftLimit >= transaction.withdrawal - account.balance)";
```

You also need to define your variables in your application.
``` csharp
    // Example with an Account and Transaction type
    var account = new Account() {        
        // define your own values here
        ...
    };
    var transaction = new Transction() {
        // define your own values here
        ...
    };
    
    // define the variables that will be passed into the CEL application.
    // the keys in the dictionary correspond to the variables defined in the CEL Expression.
    var variables = new Dictionary<string, object?>();
    variables.Add("account", account);
    variables.Add("transaction", transaction);
   

    // define the CEL Context.  This compiles the expression into a delegate.
    // you should cache this delegate because it is reusable and will avoid recompiling the expression.    
    var celProgramDelegate = celEnvironment.Compile(celExpression);

    // now execute the expression
    // you can invoke this function as many times as you want with different values for the variables.
    var result = celProgramDelegate.Invoke(variables);

    // the result is an object.  You can cast it to whatever type you expect to be returned from the expression.  In this example it is a boolean.
``` 

You can also define custom CEL functions.
``` csharp
   
    private static object? MyFunction(object?[] value)
    {
        var doubleArg = (double) value[0];
        
        //put your custom function logic here.
        //in this example, we are expecting one argument as a double type        
                
        return "some return value that can be any type you want";
    }

```

You register the custom CEL functions like this:
``` csharp
   var celEnvironment = new CelEnvironment(fileDescriptors, defaultNamespace);   

   // the first argument is the function name that will be exposed to the cel Expression
   // the second argument defines the types expected by the function
   // the third argument is the pointer to the function that will be executed
   celEnvironment.RegisterFunction("my_function", new[] { typeof(double) }, MyFunction);

   // functions must be registered before you parse the cel expression.
```


You can use the functions in an expression like this:
``` c
    var celExpression = "account.balance > my_function(account.overdraftLimit)";
```
Refer to the CEL-spec documentation for more examples.



Released under the [Apache License](LICENSE).