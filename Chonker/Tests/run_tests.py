import os

path_to_exec = os.path.join("..", "bin", "Debug", "net6.0", "Chonker")

path_to_good_tests = os.path.join("Main", "")
path_to_output = os.path.join("Main", "Output")
path_to_bad_tests = os.path.join("Error")


def run_tests():
    run_good_tests()
    run_error_tests()


def run_good_tests():
    tests = absolute_file_paths(path_to_good_tests)
    outputs = absolute_file_paths(path_to_output)

    # Remove folders
    # for filepath in tests:
    #     if not "." in filepath:
    #         tests.remove(filepath)

    print(tests)


def run_error_tests():
    return


def absolute_file_paths(directory):
    for dirpath, _, filenames in os.walk(directory):
        for f in filenames:
            yield os.path.abspath(os.path.join(dirpath, f))


run_tests()