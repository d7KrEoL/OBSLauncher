
# 🚀 OBS Launcher: Автоматический запуск OBS Studio для GTA San Andreas

[![GitHub Release](https://img.shields.io/github/v/release/kromskii2/OBSLauncher?style=for-the-badge)](https://github.com/kromskii2/OBSLauncher/releases)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=for-the-badge)](LICENSE)
[![.NET Version](https://img.shields.io/badge/.NET-6.0-%23512bd4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com)

Ваш незаменимый помощник для стриминга и записи геймплея GTA San Andreas! OBS Launcher автоматически запускает OBS Studio через 50 секунд после старта игры. Просто, удобно, эффективно!

![Пример работы](https://via.placeholder.com/800x400.png/2c3e50/ffffff?text=Screenshot+Placeholder) <!-- Замените на реальный скриншот -->

## ✨ Особенности

- ⏱️ **Автоматический запуск OBS Studio** после запуска GTA San Andreas
- ⏳ **Интеллектуальная задержка** 50 секунд перед запуском OBS
- 🛡️ **Работа с правами администратора** для надежной работы
- 📌 **Автозагрузка при старте Windows**
- 🎮 **Невидимый в игровом режиме** (работает в системном трее)
- 🔧 **Простая настройка** через контекстное меню
- 💻 **Лёгкий и быстрый** (менее 5 МБ памяти)

## 🚀 Установка

1. Скачайте последнюю версию из [раздела релизов](https://github.com/kromskii2/OBSLauncher/releases)
2. Распакуйте архив в любую папку
3. Запустите `OBSLauncher.exe` **от имени администратора** ⚠️

```bash
# Для разработчиков (требуется .NET 6.0 SDK):
git clone https://github.com/kromskii2/OBSLauncher.git
cd OBSLauncher
dotnet build
```

## 🖥️ Использование

1. После первого запуска программа добавится в автозагрузку
2. Иконка ![Tray Icon](https://via.placeholder.com/16/3498db/ffffff?text=O) появится в системном трее
3. Запустите GTA San Andreas как обычно
4. Через 50 секунд автоматически запустится OBS Studio!

### Управление через системный трей
| Действие              | Результат                     |
|-----------------------|-------------------------------|
| **Двойной клик**      | Показать окно приложения      |
| **Правый клик → Показать** | Открыть главное окно       |
| **Правый клик → Закрыть** | Полностью выйти из программы |

## ⚙️ Технические детали

### Как это работает
```mermaid
sequenceDiagram
    participant GTA as GTA San Andreas
    participant Launcher as OBS Launcher
    participant OBS as OBS Studio
    
    GTA->>Launcher: Запуск процесса
    Launcher->>Launcher: Обнаружение запуска GTA
    Launcher->>Launcher: Ожидание 50 секунд
    Launcher->>OBS: Запуск OBS Studio
    OBS-->>Launcher: Успешный запуск
```

### Требования
- Windows 10/11 (x64)
- [.NET 6.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/6.0)
- OBS Studio (установленный в стандартную директорию)
- GTA San Andreas

## 🤝 Участие в разработке

Мы приветствуем вклад в проект! Вот как вы можете помочь:

1. 🐞 Сообщайте об ошибках через [Issues](https://github.com/kromskii2/OBSLauncher/issues)
2. 💡 Предлагайте новые функции
3. 🛠️ Делайте Pull Request'ы
4. ⭐ Ставьте звезду репозиторию

### Сборка из исходников
```bash
# Клонировать репозиторий
git clone https://github.com/kromskii2/OBSLauncher.git

# Восстановить зависимости
dotnet restore

# Сборка Release версии
dotnet build -c Release
```

## ❓ Частые вопросы

**Q: Почему именно 50 секунд?**  
A: Это время позволяет игре полностью загрузиться, особенно при использовании модов.

**Q: Как изменить путь к OBS?**  
A: Отредактируйте константу `OBS_SHORTCUT_PATH` в Program.cs и пересоберите проект.

**Q: Как полностью удалить программу?**  
A: 1. Закройте программу через трей. 2. Удалите файлы. 3. Удалите ключ реестра:  
`HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\OBSLauncher`

**Q: Можно ли изменить задержку?**  
A: Да, отредактируйте константу `DELAY_SECONDS` в Program.cs

## 📜 Лицензия

Проект лицензирован под [Apache License 2.0](LICENSE).  
© 2023 kromskii2. Все права защищены.

> **С любовью к геймерам и стримерам** ❤️  
> Если программа вам помогла - поставьте звезду на GitHub! ⭐
