import os
import subprocess
import time
from pathlib import Path

tests_total = 0
tests_passed = 0

ansii_reset = "\033[0m"
ansii_bold = "\033[1m"
ansii_failure = "\033[91m"
ansii_success = "\033[92m"

path_to_exec = os.path.join("..", "bin", "Debug", "net6.0", "Chonker")

path_to_good_tests = os.path.join("Main", "")
path_to_output = os.path.join("Main", "Output", "")
path_to_bad_tests = os.path.join("Error", "")


def run_tests():
    run_good_tests()
    run_error_tests()
    print_score()


def run_good_tests():
    print(ansii_bold + "Main:" + ansii_reset)

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
        time_started = time.time()

        child = subprocess.Popen([path_to_exec, test], stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
        child.wait()

        execution_time = time.time() - time_started

        if child.returncode == 1:
            submit_result(Path(test).stem, execution_time, False)
        else:
            for output in outputs:
                if Path(output).stem in test:
                    f = open(output, "r")
                    contents = f.read()

                    if contents == child.communicate()[0].decode("utf-8"):
                        submit_result(Path(test).stem, execution_time, True)
                    else:
                        submit_result(Path(test).stem, execution_time, False)


def run_error_tests():
    print(ansii_bold + "\nError Catch:" + ansii_reset)

    tests = os.listdir(path_to_bad_tests)
    tests = get_full_paths(tests, path_to_bad_tests)

    for test in tests:
        time_started = time.time()

        child = subprocess.Popen([path_to_exec, test], stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
        child.wait()

        execution_time = time.time() - time_started

        if child.returncode == 1:
            submit_result(Path(test).stem, execution_time, True)
        else:
            submit_result(Path(test).stem, execution_time, False)


def get_full_paths(paths, additional_path):
    tmp = []
    for filepath in paths:
        tmp.append(os.getcwd() + os.sep + additional_path + filepath)

    return tmp


def submit_result(file, execution_time, success):
    print(file, end='')

    global tests_total
    global tests_passed
    tests_total += 1

    if success:
        tests_passed += 1

        print(ansii_success, end='')
        print(" " + str(round(execution_time, 2)) + "s", end='')
        print(" ✔")
    else:
        print(ansii_failure, end='')
        print(" " + str(round(execution_time, 2)) + "s", end='')
        print(" ✘")

    print(ansii_reset, end='')


def print_score():

    if tests_passed >= tests_total:
        print(ansii_success)
    else:
        print(ansii_failure)

    print(str(tests_passed) + "/" + str(tests_total) + " tests passed")


run_tests()
