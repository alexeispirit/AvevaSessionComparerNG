# SessionCompareNG

Библиотека для сравнения атрибутов элемента между сессиями AVEVA по тегу.
Есть возможность сравнения вновь созданного тега с удаленным, если известен номер сессии.

---

## Объект TagInfo

| Метод                                                                   | Результат | Описание                                                                                            |
|:------------------------------------------------------------------------|:----------|:----------------------------------------------------------------------------------------------------|
| TagInfo(STRING dbName, STRING udetName, STRING tagName, REAL sessionNo) |           | Конструктор объекта. dbName - имя БД, udetName - UDET, tagName - имя тега, sessionNo - номер сессии |
| Print()                                                                 |           | Вывод в командную строку всех атрибутов тега на момент сессии                                       |
| Attribute(STRING attributeName)                                         | STRING    | Возвращает значение атрибута attributeName в указанной сессии                                       |
| Attributes()                                                            | ARRAY     | Возвращает значения всех атрибутов в указанной сессии                                               |

## Объект Compare

| Метод                                                                   | Результат | Описание                                                                                            |
|:------------------------------------------------------------------------|:----------|:----------------------------------------------------------------------------------------------------|
| Compare(TagInfo tag1, TagInfo tag2)                                     |           | Конструктор объекта. tag1, tag2 - Информация о теге в разных сессиях                                |
| Print()                                                                 |           | Вывод в командную строку все отличающиеся между сессиями значения атрибутов                         |
| Attribute(STRING attributeName)                                         | ARRAY     | Возвращает значение атрибута attributeName в указанных в tag1, tag2 сессиях                         |
| Attributes()                                                            | ARRAY     | Возвращает значения всех атрибутов в указанных в tag1, tag2 сессиях                                 |
| Difference()                                                            | ARRAY     | Возвращает только отличающиеся значения атрибутов в указанных в tag1, tag2 сессиях                  |

## Объект ComparedTag

| Метод                                                                   | Результат | Описание                                                                                            |
|:------------------------------------------------------------------------|:----------|:----------------------------------------------------------------------------------------------------|
| ComparedTag(TagInfo tag1)                                               |           | Конструктор объекта. tag1 - Информация о теге                                                       |
| ComparedTag(TagInfo tag1, TagInfo tag2)                                 |           | Конструктор объекта. tag1, tag2 - Информация о теге в разных сессиях                                |
| ModifiedAttributes(BOOLEAN onlyModified)                                | ARRAY     | Возвращает результат сравнения атрибутов элемента. onlyModified - только измененные атрибуты        |

## Пример использования

<code>import 'C:\code\.net\SessionCompareNG\SessionCompareNG\bin\Debug\SessionCompareNG'
handle any
endhandle
using namespace 'SessionCompareNG'
!tag1 = object TagInfo('TMSCX/DIST_TMS', ':ProcessLine', '/DELTA', 96)
!att1 = !tag1.Attributes()
q var !att1[1]
\<ARRAY>
   [1]  \<STRING> 'ref'
   [2]  \<STRING> '=24593/10'
!attv = !tag1.Attribute('Name')
q var !attv
\<STRING> '/DELTA'
!tag2 = object TagInfo('TMSCX/DIST_TMS', ':ProcessLine', '/DELTA', 99)
!att2 = !tag2.Attributes()
q var !att2[1]
\<ARRAY>
   [1]  \<STRING> 'ref'
   [2]  \<STRING> '=24593/11'
!compare = object Compare(!tag1, !tag2)
!diff = !compare.Difference()
q var !diff[1]
\<ARRAY>
   [1]  \<STRING> 'ref'
   [2]  \<STRING> '=24593/10'
   [3]  \<STRING> '=24593/11'
!atts = !compare.Attributes()
q var !atts[1]
\<ARRAY>
   [1]  \<STRING> 'ref'
   [2]  \<STRING> '=24593/10'
   [3]  \<STRING> '=24593/11'
!compare.Print()
/DELTA
ref: =24593/10 -> =24593/11
:location1: Nulref -> /10-10
:from: Nulref -> /V-1800
:to: Nulref -> /V-1808
</code>
