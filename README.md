# SitemapGenerator

Русский см. ниже.

----------------------------------------------------
English

For execute require .NET Core 3.1.

Small console .NET Core application to generate the simple sitemap.xml from the local site directory

Usage
vsm.exe LocalPathToSiteRoot

1. Create "site.cfg" in the root site local directory.
In file "site.cfg" set the first line to the url of site 
See example at https://github.com/consp11/consp11.github.io/blob/master/site.cfg

2. Execute vsm.exe with one argument.
Example:
Z:\SitemapGenerator\netcoreapp3.1\vsm.exe Z:\consp11.github.io

3. Will generated vsm.cfg and sitemap.xml

4. If need.
Change priorities in vsm.cfg
String 'X: FileName' deleted file from sitemap.xml

Example:
https://github.com/consp11/consp11.github.io/blob/master/vsm.cfg

5. Execute vsm.exe again (if need)

6. If site files are changed, restart it vsm.exe
Example:
Z:\SitemapGenerator\netcoreapp3.1\vsm.exe Z:\consp11.github.io

and publish new sitemap.xml to the your site

----------------------------------------------------
Русский

Для запуска требует наличия .NET Core 3.1

Это маленький консольный генератор для генерации простых sitemap.xml по локальной директории сайта

Использование
vsm.exe УкажитеЛокальныйПутьККорнюСайта

1. Создайте в локальной корневой директории сайта файл "site.cfg".
В первой строке этого файла запишите url сайта
Например, смотрите файл https://github.com/consp11/consp11.github.io/blob/master/site.cfg

2. Запустите vsm.exe с одним аргументом.
Напрмер:
Z:\SitemapGenerator\netcoreapp3.1\vsm.exe Z:\consp11.github.io

3. Сгенерируется файл настроек vsm.cfg и файл sitemap.xml

4. Если необходимо
В файле vsm.cfg измените приоритеты для страниц
Строка вида "X: ИмяФайла" удаляет файл из sitemap.xml

Пример файла:
https://github.com/consp11/consp11.github.io/blob/master/vsm.cfg

5. Запустите vsm.exe снова (если требуется)

6. Если файлы сайта изменились, снова запустите vsm.exe
Example:
Z:\SitemapGenerator\netcoreapp3.1\vsm.exe Z:\consp11.github.io

и опубликуйте новый sitemap.xml на ваш сайт
