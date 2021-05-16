def f200():
    print("IN THE SUBPROC")
    if A > T:
        print("YOU ARE OLDER THAN ME")
    if A < T:
        print("YOU ARE YOUNGER THAN ME")
    


print("WHATS YOUR NUMBER?")
print("HOW OLD ARE YOU?")
B = int(input())
A = int(input())
# "AGE OF TINY BASIC"
T = 2021 - 1975
f200()
print("BEFORE SUBPROC")
print("AFTER SUBPROC")
