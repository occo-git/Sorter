# FileProcessingApp

## Project structure:

**`FileProcessing`** (class lib) - the main processing logic

├ `DataGeneration` - data generation logic    
├ `DataModels` - data models for processing    
├ `DataProcessing` - processing data in chunks    
└ `DataStorage` - reading/writing data (files, streams, etc.)
    
**`FileProcessing.Benchmarks`** - (class lib) - benchmarks for the main processing logic

**`FileProcessing.Tests`** - (class lib) - unit tests for the main processing logic 

**`Sorter`** - (console app) - the application for sorting files

**`TestFileGenerator`** - (console app) - the application for generating test files
