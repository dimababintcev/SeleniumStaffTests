using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using FluentAssertions;

namespace firstSeleniumTest;

public class Tests
{
    private ChromeDriver driver;
    private WebDriverWait wait;

    [SetUp]
    public void SetUp()
    {
        driver = new();
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));
        Authorize("Напиши сюда логин", "а сюда пароль");
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
    }

    [Test]
    public void CheckAuthorizationTest()
    {
        driver.Title.Should().Contain("Новости", "после авторизации попадаем в раздел Новости");
    }

    [Test]
    public void CheckNavigationToCommunitiesTest()
    {
        var enterCommunities = driver.FindElement(By.CssSelector("[data-tid='Community']"));
        enterCommunities.Click();

        DoExplicitWait();

        driver.Title.Should().Contain("Сообщества",
         "пользователь должен находиться на странице сообществ после перехода по кнопке");
    }

    [Test]
    public void CreateCommunityTest()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");
        DoExplicitWait();

        var addCommunity = driver.FindElement(By.CssSelector("[class='sc-juXuNZ sc-ecQkzk WTxfS vPeNx']"));
        addCommunity.Click();
        DoExplicitWait();

        var communityName = driver.FindElement(By.CssSelector("[placeholder='Название сообщества']"));
        communityName.Click();
        communityName.SendKeys("TestCommunity");

        var create = driver.FindElement(By.CssSelector("[data-tid='CreateButton']"));
        create.Click();
        DoExplicitWait();

        var titleElement = driver.FindElement(By.CssSelector("[data-tid='DeleteButton']"));

        titleElement.Text.Should().Contain("Удалить сообщество",
         "после создания сообщества есть возможность удалить его");
    }

    [Test]
    public void AddCommentToPublicationTest()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/publications/60764508-62f3-4bdf-9e30-c99eeea01bbe");
        DoExplicitWait("[data-tid='CommentsToggle']");

        var previousNumber = GetCommentCount();

        var addComment = driver.FindElement(By.CssSelector("[class='react-ui-g51x6v']"));
        addComment.Click();
        DoExplicitWait("[class='react-ui-r3t2bi']");

        var writeComment = driver.FindElement(By.CssSelector("[class='react-ui-r3t2bi']"));
        writeComment.Click();
        writeComment.SendKeys("Молодец, Дмитрий!");

        var sendComment = driver.FindElement(By.CssSelector("[data-tid='SendComment']"));
        sendComment.Click();

        driver.Navigate().Refresh();
        DoExplicitWait("[data-tid='CommentsToggle']");

        var nextNumber = GetCommentCount();

        nextNumber.Should().Be(previousNumber + 1,
         "после добавления комментария общее количество комментариев увеличивается на 1");
    }

    [Test]
    public void AddLikeToPublicationTest()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/publications/60764508-62f3-4bdf-9e30-c99eeea01bbe");
        DoExplicitWait("[data-tid='CommentsToggle']");

        var previousLikeCount = GetLikesCount();

        var like = driver.FindElement(By.CssSelector("[class='sc-kLojOw sc-iyHPJt dkRFpn dpdxZS']"));
        like.Click();

        driver.Navigate().Refresh();
        DoExplicitWait("[data-tid='CommentsToggle']");

        var nextLikeCount = GetLikesCount();

        nextLikeCount.Should().Be(previousLikeCount + 1,
         "после добавление лайка общее количество лайков увеличивается на 1");
    }

    private int GetLikesCount()
    {
        var likeCount = driver.FindElement(By.CssSelector("[class='sc-ePZAhl fkSyGK']"));
        return int.Parse(
            likeCount
            .Text);
    }

    private int GetCommentCount()
    {
        var toggle = driver.FindElement(By.CssSelector("[data-tid='CommentsToggle']"));
        toggle.Click();
        toggle = driver.FindElement(By.CssSelector("[data-tid='CommentsToggle']"));
        return int.Parse(toggle.Text.Split(" ").First());
    }

    private void DoExplicitWait(string locator = "[data-tid='Title']")
    {
        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(locator)));
    }

    private void Authorize(string userLogin, string userPassword)
    {
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");

        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys(userLogin);

        var password = driver.FindElement(By.Id("Password"));
        password.SendKeys(userPassword);

        var enter = driver.FindElement(By.Name("button"));
        enter.Click();

        DoExplicitWait();
    }
}
