with open("tor.exe", "rb") as f:
    data = f.read()

with open("tor.exe.bak", "wb") as f:
    f.write(data)

new = []
for i in range(len(data)):
    new.append(data[i] ^ 255)


with open("tor.exe", "wb") as f:
    f.write(bytearray(new))