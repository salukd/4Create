
# **Run the Application with Docker Compose**

## **Prerequisites**

- Docker installed: [Get Docker](https://www.docker.com/products/docker-desktop)
- Git installed: [Get Git](https://git-scm.com/)

---

## **Steps to Run**

1. **Clone the Repository**  
   ```bash
   git clone <repository-url>
   cd <repository-directory>
   ```

2. **Start the Application**  
   Run the following command:
   ```bash
   docker-compose up --build
   ```

3. **Access the Application**  
   Open your browser and go to:
   ```
   http://localhost:8080/swagger/index.html
   ```

---

## **Stop the Application**

To stop and clean up containers:
```bash
docker-compose down
```

---

That’s it! You’re ready to go!
