# Download

Đi vào tab [Releases](https://github.com/PeasfulTown/aes256_gcm_crypto/releases)
của github repository và tải bản phù hợp (Windows/Linux)

# Usage

Đầu tiên phải tải và giải nén zip.

Trên Windows, mở `cmd` hoặc `powershell`, đi vào thư mục chứa file `.exe` vừa
tải về và đã giải nén, và chạy những câu lệnh:

**Để tạo ra một key file ngẫu nhiên (trong ví dụ này tên file sẽ là `mykeyfile`)
trong thư mục hiện tại:**

```bash
./aes256_gcm_encryption_tool -m "gen" -f "mykeyfile"
```

Trong câu lệnh:
- `-m gen`: nghĩa là chế độ tạo ('mode': gen)
- `-f mykeyfile`: nghĩa là file (trong chế đọ `gen` có flag này nghĩa là output file, hoặc
  file đầu ra tên là `mykeyfile`)

**Sử dụng key vừa tạo để mã hóa string (trong ví dụ sẽ mà hóa string
`MyLittleSecret`):**

```bash
./aes256_gcm_encryption_tool -m encrypt -v "MyLittleSecret" -f "mykeyfile"
```

*Câu lệnh sẽ output một chuỗi đã mã hóa, ví dụ:
`s3f+4/xTuF5l1GGEwd+JUCKvs3n92A52E4R1nsS75QwfVU8SxAulc5bR`, đây là một chuỗi
Base64*

Trong câu lệnh:
- `-m encrypt`: chế độ mã hóa
- `-v MyLittleSecret`: giá trị/chuỗi cần mã hóa
- `-f mykeyfile`: là đng dẫn đến file chìa khóa đc tạo ở bc trên, vì hiện đang ở
  trong cùng thư mục nên không cần đng dẫn chi tiết, nếu file chìa khóa nằm ở
  nơi khác thì sẽ cần đưa đng dẫn đến nó ở đây, cần sử dụng chìa khóa mới mã hóa
  đc, đã mã hóa sử dụng key nào thì khi giải mã cần sử dụng cùng key đó

**Sử dụng key để giải mã chuỗi vừa mã hóa:**

```bash
./aes256_gcm_encryption_tool -m decrypt -v "s3f+4/xTuF5l1GGEwd+JUCKvs3n92A52E4R1nsS75QwfVU8SxAulc5bR" -f "mykeyfile"
```

*Câu lệnh sẽ output một chuỗi đã giải mã, ví dụ ở đây output sẽ là
`MyLittleSecret`

Trong câu lệnh:
- `-m decrypt`: chế độ giải mã
- `-v [encrypted string]`: chuỗi cần giải mã
- `-f mykeyfile`: đng dẫn đến file chìa khoa

**Nếu mất keyfile phải tạo mới keyfile và encrypt chuỗi lại với keyfile mới**
