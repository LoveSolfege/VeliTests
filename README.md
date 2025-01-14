ტესტებს **.NET SDK 9** სჭირდება, და თავისი პაქიჯები რომლებიც ყენდება **dotnet restore** ბრძანებით

ტესტის გასაშვებად ბრძანება **dotnet test**

გაკეთებულია მთლიანი API Flow, ასევე საძიებო ველისა და კალათის ტესტები.

API Flow განთავსებულია **Tests/ApiFlowTests.cs**-ში
საძიებო ველი - **Tests/ComponentTests/SearchBarTests.cs**
კალათა - **Tests/ComponentTests/CartTests.cs**
**სორტირება-ფილტრაციის კომპონენტების ტესტები დაუსრულებელია.**
**სურვილების სია არ გამიტესტია, ავტორიზაციას ითხოვს.**


**საძიებო ველის ტესტები:**
1. ზედმეტად მოკლე საძიებო სიტყვა
2. ცარიელი საძიებო სიტყვა
3. ადეკვატური საძიებო სიტყვა

**კალათის ტესტები:**
1. პროდუქტის დამატება
2. დამატებული პროდუქტის რაოდენობის ნეგატიური რაოდენობა
3. დამატებული პროდუქტის მარაგზე მეტი რაოდენობა

**API Flow აღწერა:**

Harcoded საძიებო სიტყვაა კოდში ("Sennheiser"), მის გამოყენებით იძებნება პროდუქტი. პროდუქტების სიას მოყვება კატეგორიები სადაც ისინი ფიგურირებენ, რანდომულად ირჩევა ერთ-ერთი კატეგორია.
შემდგომ იძებნება ყველა ფილტრი არჩეული კატეგორიისთვის და რანდომულად ირჩევა ერთი და ყენდება.
ფილტრაციის შემდეგ პროდუქტი სორტირდება ფასის კლებადობით. 

ხანდახან (უმეტესად) ისე ხდება რომ არჩეულ ფილტრი ყველა პროდუქტს აქრობს, რასაც კონსოლში ტესტი დაწერს დიდი ასოებით.

თუ ყველა პროდუქტი გაიფილტრება, არსებობს მუშა product ID, რომელიც მატდება კალათაში რანდომულად არჩეული გაფილტრულ-დასორტირებული პროდუიქტის მაგივრად. 

საბოლოოდ, მოწმდება პროდუქტის მარაგი, რანდომულად ირჩევა მარაგის ფარგლებში რიცხვი და იცვლება კალათაში პროდუქტის რაოდენობა.
