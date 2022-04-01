# Chonker

Chonker is a scripting statically-typed programming language made in C#.

If you want to learn/use it, take a look at [HowToUse.md](https://github.com/Cuber01/Chonker/blob/master/HowToUse.md).

Do note that this language was created for learning purposes and shouldn't be used in real development.
Mostly because it lacks library support and is horrifyingly slow.

# Running

To run the language you have to compile it or take a ready executable from Releases.

## Compiling

To compile run:
```sh
msbuild
```
at Chonker/Chonker

Or use your favorite IDE.

## Executing

Then simply
```sh
./Chonker
```

See [HowToUse.md](https://github.com/Cuber01/Chonker/blob/master/HowToUse.md) for commandline arguments.

# Tests

## Running

To run tests, execute `run_tests.py` at `Chonker/Chonker/Tests`
```sh
python3 run_tests.py
```

## Writing

### Normal

1. Write your test as a `yourname.chonk` file and put it at `Chonker/Chonker/Tests/Main`
2. Write the expected output of your test and put it at `Chonker/Chonker/Tests/Main/Output`, name it `yourname.out`

### Error Catch

1. Write your test as a `yourname.chonk` file and put it at `Chonker/Chonker/Tests/Error`

# Credits

Robert Nystrom for writing a [great book](http://craftinginterpreters.com/).

[Egor Dorichev](https://github.com/egordorichev/lit) for inspiration and support.

[@Cuber01](https://github.com/Cuber01) for writing this thing.

