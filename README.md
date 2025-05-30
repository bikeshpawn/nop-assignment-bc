# NopCommerce Custom Features & Deployment (BambooCard Task)

The demo for the completed task is available at : [Bamboocard-task](https://bamboocardtask.esaralaccount.com/)

**Credentials:**  
- Email: `admin@yourstore.com`  
- Password: `admin987`  

---

## 1️⃣ Custom Discount Plugin

### Installation Steps:
A discount requirement plugin is created for this task.

1. Go to **Local Plugin** in the Admin Area.
2. Look for **Custom Discount Plugin**, install and restart.
   - It will assign a discount to the total with a default discount requirement rule.

   ![Plugin Install Screenshot](https://github.com/user-attachments/assets/91e8045a-00b6-466f-96d2-b04fadd31678)

3. Now configure from the **Admin Menu** or via the plugin’s configure link.
4. Enable the plugin and enter the desired discount percentage.
5. The discount will apply at checkout if the customer has more orders than defined in the requirement rule.

   ![Discount Configuration](https://github.com/user-attachments/assets/b740b71f-cd4a-437c-aaef-d3d5ea835315)

> **Note:** “No. of Orders” refers to the total number of past orders required to apply the discount.

---

## 2️⃣ Adding Gift Message

To add a gift message at checkout:

1. Go to **Checkout Attributes**.
2. Add a new attribute named **"Gift Message"** with control type **Textbox**.

It will appear:
- On the public store’s **Checkout Cart** page.
- In the **Admin Site** under the product detail of the order.

![Gift Message Public Store](https://github.com/user-attachments/assets/9e1368e3-663e-42d2-babf-7b498f4ade02)  
![Gift Message Admin View](https://github.com/user-attachments/assets/64c69517-5fcd-47b2-aafc-eb2033b4e9df)

---

## 3️⃣ Search Attribute by Name/Keyword

Implemented a feature to search product attributes by keyword/name using **ExtendedViewExpander**.

- All customization is placed in the `Extended` folder found in:  
  `Nop.Web`, `Nop.Core`, `Nop.Data`, `Nop.Services`, and `Nop.Framework`.
- This method avoids touching the core code by using partial class overrides, enabling easy version upgrades in the future.

Check the search filter in **Product Attributes** page:

![Product Attribute Search](https://github.com/user-attachments/assets/eac50086-bf51-4a61-bc95-8ed1c6f3d878)

---

## 4️⃣ API Development (Order Retrieval)

A secure **API Module** is added with **JWT Authentication** and Swagger documentation.

### API Testing:
Visit: [Swagger API](https://bamboocardtask.esaralaccount.com/api/swagger/index.html)

1. **Generate Token**
   - Use:
     - Username: `admin@yourstore.com`
     - Password: `admin987`
   - This will provide a bearer token.

   ![Token Screenshot](https://github.com/user-attachments/assets/5d49669a-a23e-451f-a4b7-ef6748e790bd)
   ![Token Response](https://github.com/user-attachments/assets/1ff3527f-7476-4245-b2de-5f47484ac158)

2. **Authorize** using the token.
3. Use `GetOrderDetailsByEmail` endpoint with `email=admin@yourstore.com`.

   ![Order Response](https://github.com/user-attachments/assets/761fcde0-5243-4a71-9b47-99b39bc3e778)

> **Postman Collection:**  
Available in the `wwwroot` directory with name `NopCommerceApi-BambooCard.postman_collection.json
> `

---

## 5️⃣ Containerization & Quick Deployment Setup

To ensure seamless deployment of the modified nopCommerce app, this project is fully dockerized and includes setup for database, plugins, and app.

### 📦 Docker Setup Instructions

> 💡 **Requirement:** Install Docker Desktop from [here](https://www.docker.com/products/docker-desktop)

### 📁 Project Structure

In your `/src` directory, ensure the following files exist:
- `Dockerfile`
- `docker-compose.yml`
- `entrypoint.sh`

### 🏗️ Build & Run Locally

1. Open Terminal or Visual Studio Terminal.
2. Navigate to the `/src` directory.

### Clean up previous volumes (optional but recommended):
docker compose down --volumes

### Build and start containers:
docker compose up --build -d

**This will:**
- `Build images`
- `Start containers in detached mode`

**To Check Docker Images & Containers Check via terminal:**
- `docker images`
- `docker ps`

**Example Screenshots: Images**

![image](https://github.com/user-attachments/assets/26b0cb26-0ae3-4d5a-ad0b-63c7ddce9d9b)

**Example Screenshots : Containers**
![image](https://github.com/user-attachments/assets/5fbd799d-15c0-451d-b72a-ef1f0df34824)


Access the application:
Open http://localhost:8080 in your browser.

# Azure Hosting (App Service)

Here's a brief overview of hosting on Azure using App Services:

* Go to [portal.azure.com](https://portal.azure.com/#home).
* Click on **App Service** and then **Create new app**.

    ![image](https://github.com/user-attachments/assets/f9ed39f6-1dea-49a8-9952-389add2cb4a2)

* Fill in the required fields such as **Subscription**, **Region**, **Engine**, **Servername**, **Database name**, and **hosting plans**, then click **Create**.

    ![image](https://github.com/user-attachments/assets/0c40ecc4-cc30-40a8-b385-d8e675c6a4ea)

* Now, you can publish your web app directly to Azure through Visual Studio.

    ![image](https://github.com/user-attachments/assets/d108a448-102e-42bb-bfb3-5c74c0cc69bf)
![image](https://github.com/user-attachments/assets/8c6365ed-c442-4482-8104-4c91096899d9)

---

# Azure Hosting CI/CD

This is the deployment mechanism I'm currently working with. It consists of **pipelines** (specifically, build pipelines) that generate artifacts, and **releases** that take these artifacts and deploy them to the necessary environments.
**Example Screenshots: Pipelines**
![image](https://github.com/user-attachments/assets/d7bafac8-1295-410b-b3d4-08eb9aee0ebd)

**Example Screenshots: Releases**
![fff](https://github.com/user-attachments/assets/043219b8-8cb9-4406-99f6-33780d7ab540)
