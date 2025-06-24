# SessionCompareNG

Библиотека для сравнения атрибутов элемента между сессиями AVEVA по тегу.
Есть возможность сравнения вновь созданного тега с удаленным, если известен номер сессии.

---

## Методы

| Метод                                             | Результат | Описание                                                                                |
|:--------------------------------------------------|:----------|:----------------------------------------------------------------------------------------|
| Run(STRING tag, REAL sess1, REAL sess2)           |           | Запуск сравнения                                                                        |
| Print()                                           |           | Вывод результата сравнения в командную строку                                           |
| Attribute(STRING attributeName)                   | ARRAY     | Возвращает различия значения атрибута attributeName между сессиями                      |
| Attributes()                                      | ARRAY     | Возвращает различия значения атрибутов между сессиями                                   |

## Пример использования

<code>import 'C:\code\.net\SessionCompareNG\SessionCompareNG\bin\Release\SessionCompareNG'
handle any
endhandle
using namespace 'SessionCompareNG'
!sc = object SessionCompareNG()
!sc.Run('/DELTA', 96, 99)
!all = !sc.Attributes()
!one = !sc.Attribute('ref')
!sc.Print()
/DELTA
REF: =24593/10 -> =24593/11
:Location1: Nulref -> /10-10
:From: Nulref -> /V-1800
:To: Nulref -> /V-1808</code>
