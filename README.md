# Witch-adventure - 2D Action Game
**Witch adventure ** — це 2D гра з топ-даун виглядом, де ви граєте за відьму, що бореться зі скелетами на невеликій карті. Гравець використовує меч, щоб перемагати ворогів, досліджувати ліс і проходити різні випробування на карті.


## Особливості
- **Топ-даун вид:** класичний погляд зверху на карту.
- **Бойова система:** використання меча для знищення скелетів.



## Управління
- **Рух:** `W` (вгору), `A` (вліво), `S` (вниз), `D` (вправо)  
- **Атака:** Ліва кнопка миші (LMB)  
- **Ривок:** `Shift`


## Спринт
- 1. Зробити героя який може рухатись,бити мечом і померти; Зробити ворога який ходить, наносить гравцю йому шкоду (бється з ним) і може помери; Зробити невеличку карту з парочкою обєктів.
- 2. Зробити ривок герою і привязати до нього камеру; Зробити більшу карту за допомогою Tilemap; Зробити кущ який можна зламати.

     
## Trello дошка

- **Backlog (Ідеї/Плани)**
- **Картки (Cards / Issues)**
-  Додати інтерфейс
-  Додати інвентар
-  Додати ще ворогів
-  Додати ще зброї
-  Зробити лут з обєктів та ворогів
  
- **To Do (До виконання)**
-  Переробка карти, поліпшення
-  Добавлення переходу між локаціями

- **In Progress (В роботі)**
-  Прозорість дерев при заході за нього

- **Testing (Тестування)**
- Tilemap
- Navmesh в новому середовищі

- **Done (Готово)**
-  Здоровя героя
-  Рух героя
-  Камера
-  Меч
-  Скелет
-  Знищуймий обєкт
-  Карта
-  Navmesh


## Юніт-тести

```csharp
using NUnit.Framework;
using UnityEngine;
using System.Collections;

public class PlayerDamageTests
{
    private GameObject playerObj;
    private Player player;
    private KnockBack knockBack;
    private TrailRenderer trail;
    private GameObject cameraObj;
    private GameObject gameInputObj;

    [SetUp]
    public void Setup()
    {
        // Створюємо Player
        playerObj = new GameObject();
        playerObj.AddComponent<Rigidbody2D>();
        player = playerObj.AddComponent<Player>();

        // Додаємо KnockBack
        knockBack = playerObj.AddComponent<KnockBack>();

        // Додаємо TrailRenderer
        trail = playerObj.AddComponent<TrailRenderer>();
        typeof(Player).GetField("trailRenderer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(player, trail);

        // Створюємо камеру для Player
        cameraObj = new GameObject();
        cameraObj.AddComponent<Camera>();

        // Створюємо мок GameInput
        gameInputObj = new GameObject();
        gameInputObj.AddComponent<GameInput>();

        // Викликаємо Start Player через Reflection
        typeof(Player).GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(player, null);
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(playerObj);
        GameObject.DestroyImmediate(cameraObj);
        GameObject.DestroyImmediate(gameInputObj);
    }

    [Test]
    public void TakeDamage_ReducesHealth()
    {
        Transform dummySource = new GameObject().transform;

        // Викликаємо TakeDamage
        player.TakeDamage(dummySource, 30);

        // Через Reflection отримуємо _currentHealth
        int currentHealth = (int)typeof(Player).GetField("_currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(player);

        Assert.AreEqual(70, currentHealth, "TakeDamage не зменшив здоров'я правильно");
    }

    [Test]
    public void TakeDamage_PlayerDiesAtZeroHealth()
    {
        Transform dummySource = new GameObject().transform;

        // Викликаємо TakeDamage більше, ніж здоров'я
        player.TakeDamage(dummySource, 150);

        Assert.IsFalse(player.IsAlive(), "Player не помер при здоров'ї <= 0");
    }

    [Test]
    public void TakeDamage_HealthDoesNotGoBelowZero()
    {
        Transform dummySource = new GameObject().transform;

        player.TakeDamage(dummySource, 200);

        int currentHealth = (int)typeof(Player).GetField("_currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(player);

        Assert.AreEqual(0, currentHealth, "Здоров'я не повинно бути нижче 0");
    }
}
```


## Автор
Федунь Артур
