# Chatbees Engine

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/5438f54dfa684f20862170acef53255d)](https://www.codacy.com/manual/chatbee/Engine?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=chatbee/Engine&amp;utm_campaign=Badge_Grade)
[![Build Status](https://dev.azure.com/chatbees/platform/_apis/build/status/chatbee.Engine?branchName=master)](https://dev.azure.com/chatbees/platform/_build/latest?definitionId=2&branchName=master)
![Nuget version](https://img.shields.io/nuget/v/chatbeeengine)
![Code Coverage](https://img.shields.io/azure-devops/coverage/chatbees/platform/2/master)

This package is built as a general purpose automation engine. It currently powers our Chatbot as a Service platform. 

## Usage

The usage is in two parts, essentially. First, you will use `ITask`, `IStartTask`, and `IErrorHandlingTask` to implement your work flow in a 'linked-list' fashion. The execution starts with an `IStartTask` and will keep following the id pointers. 

The underlying implementation of the tasks is up to you, but you will form a `JobConfiguration`

Once this configuration is built, simply use the `EngineService` to begin an instance of execution using `NewInstance`. 

Then you can process the input using the Guid returned. 

> note: Due to some issues with reflection, all types must be registered with `RegisterTypes`
