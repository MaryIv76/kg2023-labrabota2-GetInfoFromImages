# Автор
**Иванова Мария, 13 гр.**

# Предмет 
Компьютерная графика. Лабораторная работа 2. Чтение информации из графических файлов

# Запуск приложения
В папке [Lab2_exe](https://github.com/MaryIv76/kg2023-labrabota2-GetInfoFromImages/tree/main/Lab2_exe)
лежит файл Lab2.exe, а также библиотеки, необходимые для работы exe-файла.

По нажатию на файл Lab2.exe можно запустить готовое приложение.

# Установка приложения
1. Клонировать репозиторий

```
git clone https://github.com/MaryIv76/kg2023-labrabota2-GetInfoFromImages.git
```

2. Перейти в папку Lab2

```
cd Lab2/
```

3. Собрать проект, используя msbuild (**про msbuild смотреть ниже!**)

```
"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" "Lab2.csproj" /p:Configuration=Release /p:Platform=AnyCPU
```

4. Lab2.exe будет создан в папке ```Lab2\bin\Release\```

### MSBuild
1. MSBuild можно найти в папке C:\Windows\Microsoft.NET\Framework.

Путь приблизительно такой: 

```C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe```

.NET Framework можно скачать [здесь](https://dotnet.microsoft.com/en-us/download/dotnet-framework)

2. Если установлена Visual Studio 2022, то средства сборки устанавливаются в папку установки Visual Studio.
Файл MSBuild.exe находится в папке установки MSBuild\Current\Bin.

Путь приблизительно такой: 

```C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin\MSBuild.exe```

В более ранних версиях Visual Studio расположение MSBuild может быть немного другим.

# Описание
Данное приложение является графическим приложением на WPF (Windows Presentation Foundation), написанным с помощью языка программирования C#.

Приложение считывает из графического файла/файлов или папки, содержащей графические файлы,
основную информацию об изображении, а именно:
* имя файла;
* размер изображения (в пикселях);
* разрешение (dot/inch);
* глубину цвета;
* сжатие.

Обрабатываемые форматы: jpg, jpeg, gif, tif, tiff, bmp, png, pcx.

# Сопроводительная документация
Для получения информации из изображений использовались классы `Bitmap` (`System.Drawing`) и `Image` (`System.Drawing`).

С помощью Bitmap были получены данные о размере изображения, вертикальном и горизонтальном разрешении, глубине цвета. Например, получить размер изображения:
```
Bitmap bitmap = new Bitmap(fileName);
string size = bitmap.Width + " x " + bitmap.Height;
```

С помощью Image было получено сжатие изображения:
```
Image image = Image.FromFile(fileName);
PropertyItem propItem = image.GetPropertyItem(259);
int compressionId = BitConverter.ToInt16(propItem.Value, 0);
```

Для получения информации из графических файлов с расширением .pcx была использована библиотека MetadataExtractor. С её помощью были получены данные о размере изображения, вертикальном и горизонтальном разрешении, глубине цвета. Например, получение горизонтального разрешения:
```
var directories = ImageMetadataReader.ReadMetadata(fileName);
var pcxFileData = new Dictionary<string, string>();

foreach (var directory in directories)
{
  foreach (var tag in directory.Tags)
  {
    pcxFileData[string.Format("{0} - {1}", directory.Name, tag.Name)] = tag.Description;
  }
}

string horizontalResolution = pcxFileData["PCX - Horizontal DPI"];
```

Сжатие .pcx изображения было получено через байтовое представление файла:
```
byte[] bytes = new byte[3];
var reader = new BinaryReader(new FileStream(fileName, FileMode.Open));
reader.Read(bytes, 0, 3);
```

