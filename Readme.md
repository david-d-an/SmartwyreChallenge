# Notes for Evaluator

## To run the project
The service project is added to the Runner poject. Please follow the script below to run it.
```
$ cd Smartwyre.DeveloperTest.Runner
$ dotnet run
```

## To execute unit tests

Follow instructions [here](https://jasonwatmore.com/net-vs-code-xunit-setup-unit-testing-code-coverage-in-aspnet-core) to set up a general test environment.

The service project is testable by unit tests. Please follow the script below to run it.
```
$ cd Smartwyre.DeveloperTest.Tests
$ dotnet test
```

To generate a coverage report, run `generate coverage report` task.
```
ctrl+shift+p > Tasks: Run Task > generate coverage report.
```

The current Test Coverage should be posted [here](./Smartwyre.DeveloperTest.Tests/TestResults/coveragereport/index.htm). Use a web browser to see the coverage details.


# Smartwyre Developer Test Instructions

In the 'RebateService.cs' file you will find a method for calculating a rebate. At a high level the steps for calculating a rebate are:

 1. Lookup the rebate that the request is being made against.
 2. Lookup the product that the request is being made against.
 2. Check that the rebate and request are valid to calculate the incentive type rebate.
 3. Store the rebate calculation.

What we'd like you to do is refactor the code with the following things in mind:

 - Adherence to SOLID principles
 - Testability
 - Readability
 - In the future we will add many more incentive types. Determining the incentive type should be made as easy and intuitive as possible for developers who will edit this in the future.

We’d also like you to 
 - Add some unit tests to the Smartwyre.DeveloperTest.Tests project to show how you would test the code that you’ve produced 
 - Run the RebateService from the Smartwyre.DeveloperTest.Runner console application accepting inputs

The only specific 'rules' are:

- The solution should build
- The tests should all pass

You are free to use any frameworks/NuGet packages that you see fit. You should plan to spend around 1 hour completing the exercise.
