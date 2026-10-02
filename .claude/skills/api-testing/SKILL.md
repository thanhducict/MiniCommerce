---
name: api-testing
description: Test MiniCommerce.Web REST APIs using real HTTP requests. Use when testing Product API endpoints or other backend API workflows.
---

# API Testing

## Purpose

Test the REST APIs exposed by `MiniCommerce.Web`.

Use real HTTP requests against the running application.

Do not modify application code while testing unless explicitly requested.

## Starting the Application

Before testing:

1. Check whether `MiniCommerce.Web` is already running.
2. If it is not running, start it with:

```powershell
dotnet run --project MiniCommerce.Web
```

3. Determine the HTTP URL and port from the application output or project configuration.
4. Use that URL as the API base URL.

## General Test Rules

For every endpoint:

1. Send a real HTTP request.
2. Verify the HTTP status code.
3. Verify the response body when applicable.
4. Use data returned from previous requests when possible instead of hard-coding IDs.
5. Report each test as PASS or FAIL.

When a test fails, report:

- HTTP method
- URL
- Expected result
- Actual result

Do not modify the application to make a test pass.

## Product API

The Product API is available under:

```text
/products
```

### 1. Get All Products

```http
GET /products/
```

Verify that:

- The request succeeds.
- The response contains a collection of products.
- The returned products contain the expected product information.

Use a product ID from this response for subsequent tests where appropriate.

### 2. Get Product by ID

```http
GET /products/{productId}
```

Use an existing product ID.

Verify that:

- The request succeeds.
- The returned product has the requested `ProductId`.

Also test a non-existing product ID.

Verify that the API returns the expected not-found response.

### 3. Get Products by Category

```http
GET /products/category/{categoryName}
```

Use a category that exists in the product data.

Verify that:

- The request succeeds.
- The response contains products belonging to the requested category.

Also consider testing a category that does not exist.

### 4. Create Product

```http
POST /products/
```

Send a valid `Product` JSON object.

Verify that:

- The request succeeds with the expected creation status.
- A product ID is returned/created.
- The created product can be retrieved.

Keep track of the created product ID so it can be used by later tests.

### 5. Update Product

```http
PUT /products/{productId}
```

Use the product created during the create test when possible.

Send an updated `Product` JSON object.

Verify that:

- The request succeeds with the expected update status.
- The product can be retrieved afterwards.
- The updated values are reflected.

Also test updating a non-existing product ID.

### 6. Delete Product

```http
DELETE /products/{productId}
```

Use the product created for testing.

Verify that:

- The request succeeds with the expected deletion status.
- The product can no longer be retrieved.

Also test deleting a non-existing product ID.

## Product API Workflow

When the user asks:

> Use the api-testing skill to test the Product API.

Execute the following workflow:

1. Start `MiniCommerce.Web` if necessary.
2. Determine the API base URL.
3. Get all products.
4. Select an existing product ID.
5. Get that product by ID.
6. Test getting a non-existing product.
7. Find a valid category from the available product data and test the category endpoint.
8. Create a test product.
9. Get the created product.
10. Update the created product.
11. Get the updated product and verify the changes.
12. Delete the test product.
13. Verify that the deleted product no longer exists.
14. Test the not-found behavior for update/delete where appropriate.
15. Report the results.

## Test Data

Prefer data discovered from the API over hard-coded values.

For create/update tests:

- Use a clearly identifiable test product.
- Avoid modifying existing production/sample products when a newly created test product can be used instead.
- Clean up test data after the test by deleting products created specifically for testing.

## Result Format

Report the result in a concise format:

```text
Product API Test Results

[PASS] GET /products/
[PASS] GET /products/{productId}
[PASS] GET /products/{invalidProductId}
[PASS] GET /products/category/{categoryName}
[PASS] POST /products/
[PASS] PUT /products/{productId}
[PASS] DELETE /products/{productId}

Summary: 7 passed, 0 failed
```

For failures, include the relevant request, expected result, and actual result.