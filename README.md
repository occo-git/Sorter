# FileProcessingApp.sln

## Project structure:
    
**`FileProcessing.Benchmarks`** - (class lib) - benchmarks for the main processing logic

**`FileProcessing.Tests`** - (class lib) - unit tests for the main processing logic 

**`FileProcessing`** (class lib) - the main processing logic

├ `DataGeneration` - data generation logic    
├ `DataModels` - data models for processing    
├ `DataProcessing` - processing data in chunks    
└ `DataStorage` - reading/writing data (files, streams, etc.)

**`Sorter`** - (console app) - the application for **sorting** files

**`TestFileGenerator`** - (console app) - the application for **generating** test files



## Generate files:
Set **`TestFileGenerator`** as startup project

Run the application to generate test files:

        Test file generator
        File name: d:/_data1
        File size: 1073741824 B
        File count: 1
        
        Choose an option:
        N - New parameters
        G - Generate
        Q - Quit
        
Press **`G`** to start generating files with the specified parameters:

        Generate 1 file: d:/_data1, Size = 1073741824
        File deleted: d:/_data1.txt
        Generating file: d:/_data1.txt
        Generate chunk: Size = 268435456, Iteration = 0/4
                Write data: Size = 268435450
        Generate chunk: Size = 268435456, Iteration = 1/4
                Write data: Size = 268435443
        Generate chunk: Size = 268435456, Iteration = 2/4
                Write data: Size = 268435434
        Generate chunk: Size = 268435456, Iteration = 3/4
                Write data: Size = 268435445
        Generate chunk: Size = 52, Iteration = 4/4
                Write data: Size = 23
        Elapsed time: 12402 ms
        
        Press any key to continue...

As a result, the program has generated `d:/_data1.txt` containing random records in the specified format `<Number>. <String>`:

    133495000. Minima ipsa sint qui debitis ad.
    1355726170. Voluptate minus dignissimos quia.
    543623703. Nesciunt cumque dolor et nesciunt consequatur quo.
    483568311. Cupiditate omnis quibusdam quod sed numquam maiores.
    280774683. Hic non libero deleniti.
    594304100. Et molestias nam aperiam qui vel laborum.
    1725587880. Quos sit.
    147075468. Qui aut.
    ...

File count > 1 means that multiple files will be generated _asyncronously_:

        d:/_data1_1.txt
        d:/_data1_2.txt
        d:/_data1_3.txt
        ....
        
## Sort files:
Set **`Sorter`** as startup project

Run the application to generate test files:
        
        Sorter
        File name: d:/_data1
        File count: 1
        
        Choose an option:
        N - New parameters
        S - Sort
        Q - Quit

Press **`S`** to start sorting files with the specified parameters:

        Sort 1 file: d:/_data1
        Processing file: d:/_data1.txt
        Processing chunk 0: Bytes = 268435450
                Ordered chunk data by Hash: Lines = 5673934
                Write data: Size = 268435450
                Temp file saved: C:\Users\Alexander\AppData\Local\Temp\chunk_724b265d2297484cbe3318a8db377a64.tmp
        Processing chunk 1: Bytes = 268435443
                Ordered chunk data by Hash: Lines = 5672668
                Write data: Size = 268435443
                Temp file saved: C:\Users\Alexander\AppData\Local\Temp\chunk_c1c5f5c1a2814645872357440bc3782c.tmp
        Processing chunk 2: Bytes = 268435434
                Ordered chunk data by Hash: Lines = 5675051
                Write data: Size = 268435434
                Temp file saved: C:\Users\Alexander\AppData\Local\Temp\chunk_20d9172133974246a7b8deeec3cd8820.tmp
        Processing chunk 3: Bytes = 268435445
                Ordered chunk data by Hash: Lines = 5672816
                Write data: Size = 268435445
                Temp file saved: C:\Users\Alexander\AppData\Local\Temp\chunk_c7eb0362adfa4af0946c530aff270171.tmp
        Processing chunk 4: Bytes = 23
                Ordered chunk data by Hash: Lines = 1
                Write data: Size = 23
                Temp file saved: C:\Users\Alexander\AppData\Local\Temp\chunk_ddef069a79a94225b0172e88c1435280.tmp
        Merging files
                Write data: Size = 268435435
                Write data: Size = 268435410
                Write data: Size = 268435398
                Write data: Size = 268435452
                Write data: Size = 100
        >> File created: d:\_data1_sorted_53d96239e0764623ad5b1e21056ba67e.txt
        Deleting temp files
        File deleted: C:\Users\Alexander\AppData\Local\Temp\chunk_724b265d2297484cbe3318a8db377a64.tmp
        File deleted: C:\Users\Alexander\AppData\Local\Temp\chunk_c1c5f5c1a2814645872357440bc3782c.tmp
        File deleted: C:\Users\Alexander\AppData\Local\Temp\chunk_20d9172133974246a7b8deeec3cd8820.tmp
        File deleted: C:\Users\Alexander\AppData\Local\Temp\chunk_c7eb0362adfa4af0946c530aff270171.tmp
        File deleted: C:\Users\Alexander\AppData\Local\Temp\chunk_ddef069a79a94225b0172e88c1435280.tmp
        Elapsed time: 97741 ms
        
        Press any key to continue...

As a result, the program has created the ordered version of `d:\_data1.txt`

`d:\_data1_sorted_53d96239e0764623ad5b1e21056ba67e.txt`:
    
    1626488818. A a a et id voluptas.
    1261667396. A a a nihil dolorem eum doloremque illo.
    1548703040. A a a officia.
    411753444. A a a ratione tenetur dicta mollitia eius alias.
    789064575. A a ab vel quia.
    253006587. A a accusamus accusamus.
    2000168013. A a accusamus dolores sint magnam.
    1632854993. A a accusamus et.
    40284948. A a adipisci maiores aut facere numquam veritatis repudiandae.
    2091809826. A a adipisci nobis.
    1587846566. A a alias iste debitis mollitia sapiente doloremque.
    ...
    
File count > 1 means that multiple files will be sorted _asyncronously_:

        d:/_data1_1.txt
        d:/_data1_2.txt
        d:/_data1_3.txt
        ....

## Sorting algorithm:

`Hash` is the property of a record (data model) for sorting and comparison

Step 1: Splitting

                            Input file (1 GB)
                                    |
                                    v
                       +-------------------------+
                       | Sequential parsing data |
                       | into record chunks      |
                       +-------------------------+
                                    |
                                    v
            +---------+  +---------+  +---------+  +---------+
            | Chunk 1 |  | Chunk 2 |  | Chunk 3 |  | Chunk 4 |
            | (256MB) |  | (256MB) |  | (256MB) |  | (256MB) |
            +---------+  +---------+  +---------+  +---------+

Step 2: Sorting and Saving to Temporary Files
        
        Chunk 1 --> Sort in memory by Hash --> guid1.tmp (sorted)
        Chunk 2 --> Sort in memory by Hash --> guid2.tmp (sorted)
        Chunk 3 --> Sort in memory by Hash --> guid3.tmp (sorted)
        Chunk 4 --> Sort in memory by Hash --> guid4.tmp (sorted)

Step 3: Multi-way Merge

              guid1.tmp   guid2.tmp   guid3.tmp   guid4.tmp
                 |           |           |           |
                 v           v           v           v
        +-------------------------------------------------------+
        |  Multi-way merge sorted files:                        |
        |  1. Read the first record from each file              |
        |  2. Select the minimum record by Hash                 |
        |  3. Read the following record                         |
        |  4. Batch selected records in memory                  |
        |  5. Write batch to the resulting file                 |
        |  6. Repeat the process until all files are exhausted  |
        |                                                       |
        |          ↓ Sequential batch recording ↓               |
        +-------------------------------------------------------+
                                  |
                                  v
                      Output file (1 GB, sorted)
                                  
                     +--------------------------+
                     | Deleting temporary files |
                     +--------------------------+
