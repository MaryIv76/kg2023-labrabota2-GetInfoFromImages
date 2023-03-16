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
"C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin\MSBuild.exe" "Lab2.csproj"
```

Если установлена Visual Studio 2022, то средства сборки устанавливаются в папку установки Visual Studio.
Файл MSBuild.exe находится в папке установки MSBuild\Current\Bin.

Путь приблизительно такой: C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin\MSBuild.exe

Если нет Visual Studio, то MSBuild можно установить, используя "Средства сборки для Visual Studio 2022" (Build Tools for Visual Studio 2022)
[https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/)

# Описание
Данное приложение является графическим приложением на WPF (Windows Presentation Foundation), написанным с помощью языка программирования C#.

Приложение считывает из графического файла/файлов или папки, содержащей графические файлы,
основную информацию об изображении, а именно:
* имя файла;
* размер изображения (в пикселях);
* разрешение (dot/inch);
* глубину цвета;
* сжатие.
