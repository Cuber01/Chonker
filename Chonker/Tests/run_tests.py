import os
import subprocess

path_to_exec = os.path.join("..", "bin", "Debug", "net6.0", "Chonker")

path_to_good_tests = os.path.join("Main", "")
path_to_output = os.path.join("Main", "Output", "")
path_to_bad_tests = os.path.join("Error", "")


def run_tests():
    run_good_tests()
    run_error_tests()


def run_good_tests():
    tests = os.listdir(path_to_good_tests)
    outputs = os.listdir(path_to_output)

    # Remove folders
    for filepath in tests:
        if not ("." in filepath):
            tests.remove(filepath)

    tests = get_full_paths(tests, path_to_good_tests)
    outputs = get_full_paths(outputs, path_to_output)

    # Run tests
    for test in tests:
        result = subprocess.Popen([path_to_exec, test], stdin=subprocess.PIPE, stdout=subprocess.PIPE)
        # print(result.communicate())


def run_error_tests():
    return


def get_full_paths(paths, additional_path):
    tmp = []
    for filepath in paths:
        tmp.append(os.getcwd() + os.sep + additional_path + filepath)

    return tmp


run_tests()
