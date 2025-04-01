# SUT COURSE API (.NET 9 version) 📚📒

This project scrapes course data from Suranaree University of Technology's registration system.

## Getting Started 🚀

Clone this repository:

```zsh
git clone https://github.com/Taweechaikxmm/sut-course-api-dotnet.git

cd sut-course-api-dotnet
```

### Start the Project

```zsh
dotnet run
```

### Start with Docker (Optional)

Build and run with Docker:

```sh
docker build -t sut-course-api-dotnet .
docker run -p 8080:8080 sut-course-api-dotnet
```
## API Reference

#### Get Courses

```http
  POST /api/courses
```

| Key                   | Type     | Description                                                                                                                       | Example           |
| --------------------- | -------- | --------------------------------------------------------------------------------------------------------------------------------- | ----------------- |
| `acadYear`            | `string` | Academic year (e.g., 2567)                                                                                                        | "2567"            |
| `semester`            | `string` | Semester (e.g., 3)                                                                                                                | `3`               |
| `courseCode`          | `string` | Course code pattern (e.g., "103122")                                                                                              | "103122"          |
| `courseName`          | `string` | Course name pattern (e.g., "english\*")                                                                                           | "english\*"       |
| `LEVELID`             | `string` | Education level for filtering courses. The available options are:                                                               |                   |
|                       |          | "1" - Bachelor's Degree (Undergraduate)                                                                                           | "1"               |
|                       |          | "2" - Master's Degree                                                                                                             | "2"               |
|                       |          | "3" - Doctoral Degree                                                                                                             | "3"               |
|                       |          | "4" - Bachelor's Degree (Special Program)                                                                                        | "4"               |
|                       |          | "5" - Graduate Diploma                                                                                                            | "5"               |
|                       |          | "6" - Advanced Graduate Diploma                                                                                                   | "6"               |
|                       |          | "11" - Additional Bachelor's Degree                                                                                               | "11"              |
|                       |          | "21" - Joint Bachelor's Degree                                                                                                    | "21"              |
|                       |          | "22" - Joint Master's Degree                                                                                                      | "22"              |
|                       |          | "31" - Bachelor's Degree (Credit Bank)                                                                                           | "31"              |
|                       |          | "32" - Master's Degree (Credit Bank)                                                                                             | "32"              |
|                       |          | "33" - Doctoral Degree (Credit Bank)                                                                                             | "33"              |
|                       |          | "41" - Learner (Credit Bank)                                                                                                     | "41"              |
|                       |          | "42" - Master's Degree (Credit Bank) (Learner) - Cancelled                                                                       | "42"              |
|                       |          | "43" - Doctoral Degree (Credit Bank) (Learner) - Cancelled                                                                       | "43"              |
|                       |          | "61" - Certificate                                                                                                               | "61"              |
|                       |          | "91" - International Program Student                                                                                              | "91"              |
|                       |          | "99" - Applied for a Program                                                                                                      | "99"              |
|                       |          | "100" - Test 14 Terms                                                                                                            | "100"             |
| `maxRow`              | `string` | Max rows in response (default is 50)                                                                                              | `25`              |
| `facultyid`(optional)        | `string` | Faculty ID for filtering courses. The available options are: |                              |
|                    |          | "all" - All faculties                                   | "all"                        |
|                    |          | "10000" - Suranaree University of Technology           | "10000"                      |
|                    |          | "10100" - School of Science                            | "10100"                      |
|                    |          | "10200" - School of Social Technology                  | "10200"                      |
|                    |          | "10300" - School of Agricultural Technology            | "10300"                      |
|                    |          | "10600" - School of Medicine                           | "10600"                      |
|                    |          | "10700" - School of Engineering                        | "10700"                      |
|                    |          | "10800" - School of Nursing                            | "10800"                      |
|                    |          | "10900" - School of Dentistry                         | "10900"                      |
|                    |          | "11000" - School of Public Health                     | "11000"                      |
|                    |          | "11100" - School of Digital Arts and Science          | "11100"                      |
| `cmd (optional)`      | `string` | Filter by day and times (`1`) or no filter (`2`)                                                                                  | `1` or `2`        |
| `weekdays (optional)` | `string` | Weekday for filtering courses. Values correspond to days of the week: "1" = Sunday, "2" = Monday, ..., "7" = Saturday. Required if `cmd` is `1`. | "2" (Monday) |
| `timeFrom (optional)` | `string` | Starting time for filtering courses. The value represents 5-minute intervals, where "1" = 00:00, "2" = 00:05, ..., "288" = 23:55. | "97" (08:00)      |
| `timeTo (optional)`   | `string` | Ending time for filtering courses. The value follows the same 5-minute interval format.                                           | "144" (12:00)     |


> **Warning:** If `courseCode` and `courseName` are not specified, scraping all data may take a long time.

### Example Requests

#### Retrieve Course Data (No Filtering)

```json
{
    "facultyid": "all",
    "maxrow": "50",
    "acadyear": "2567",
    "semester": "3",
    "CAMPUSID": "",
    "LEVELID": "1",
    "coursecode": "103122",
    "coursename": "*",
    "cmd": "2"
}
```

#### Retrieve Course Data with Filtering

```json
{
    "facultyid": "all",
    "maxrow": "50",
    "acadyear": "2567",
    "semester": "3",
    "CAMPUSID": "",
    "LEVELID": "1",
    "coursecode": "103122",
    "coursename": "*",
    "cmd": "1",
    "weekdays": "2",
    "timefrom": "1",
    "timeto": "288"
}
```

## Sample Response List

```json
[
    {
        "courseCodeVersion": " 103122 - 1 ",
        "courseName": "ANALYTICAL  CALCULUS II( หลักสูตรปรับปรุง พ.ศ. 2560)ผู้ช่วยศาสตราจารย์ ดร.ภาณุ ยิ้มเมือง",
        "professors": [
            "ผู้ช่วยศาสตราจารย์ ดร.ภาณุ ยิ้มเมือง"
        ],
        "credit": " 4 (4-0-8)",
        "language": " TH : ไทย",
        "degree": " ปริญญาตรี ",
        "schedule": "Mo16:00-18:00 B6104-ATu16:00-18:00 B6104-AWe10:00-12:00 B1118",
        "totalSeats": "1 ",
        "registered": "10 ",
        "remainingSeats": "1 ",
        "status": "9 "
    },
    {
        "courseCodeVersion": " 1101061 - 1 ",
        "courseName": "VECTOR  GRAPHIC  DESIGN( สำหรับหลักสูตรปรับปรุง พ.ศ. 2563)ผู้ช่วยศาสตราจารย์ ดร.ธวัชพงษ์ พิทักษ์นางสาวสุกัญญา แซ่เลี้ยงนายคณิต วัฒนาวงศ์ดอน",
        "professors": [
            "ผู้ช่วยศาสตราจารย์ ดร.ธวัชพงษ์ พิทักษ์",
            "นางสาวสุกัญญา แซ่เลี้ยง",
            "นายคณิต วัฒนาวงศ์ดอน"
        ],
        "credit": " 3 (2-2-5)",
        "language": " TH : ไทย",
        "degree": " ปริญญาตรี ",
        "schedule": "Mo08:00-12:00 DIGITAL TECH LAB ANIMATION 3",
        "totalSeats": "1 ",
        "registered": "50 ",
        "remainingSeats": "49 ",
        "status": "1 "
    },
    {
        "courseCodeVersion": " 1101061 - 1 ",
        "courseName": "VECTOR  GRAPHIC  DESIGN( ปี 2 เทคโนโลยีดิจิทัล หลักสูตรปรับปรุง พ.ศ. 2563)ผู้ช่วยศาสตราจารย์ ดร.ธวัชพงษ์ พิทักษ์นางสาวมาริษา โมรานอกนายจิตรทิวัส เปรมฤทัยรัตน์นายคณิต วัฒนาวงศ์ดอน",
        "professors": [
            "ผู้ช่วยศาสตราจารย์ ดร.ธวัชพงษ์ พิทักษ์",
            "นางสาวมาริษา โมรานอก",
            "นายจิตรทิวัส เปรมฤทัยรัตน์",
            "นายคณิต วัฒนาวงศ์ดอน"
        ],
        "credit": " 3 (2-2-5)",
        "language": " TH : ไทย",
        "degree": " ปริญญาตรี ",
        "schedule": "Mo08:00-12:00 DIGITAL TECH LAB ANIMATION 2",
        "totalSeats": "2 ",
        "registered": "54 ",
        "remainingSeats": "50 ",
        "status": "4 "
    },
]
```

## Tech Stack
This project is built using the following technologies:
- [.NET 9](https://dotnet.microsoft.com/)  - The latest version of the .NET platform
- [FastEndpoints](https://fast-endpoints.com/) - A high-performance web API framework for .NET
- [HtmlAgilityPack](https://html-agility-pack.net/) - An HTML parser for .NET applications
- [Newtonsoft.Json](https://www.newtonsoft.com/json) - A popular JSON framework for .NET

---

## 🚀 Enjoy Coding  
This project is inspired by [go-sut-course-api](https://github.com/pandakn/go-sut-course-api/tree/main).  
Special thanks to the contributors of the original project for their work!  
