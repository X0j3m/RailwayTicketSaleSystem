FROM node:20-alpine AS builder

WORKDIR /app
COPY src/frontend/frontendservice/package*.json ./
RUN npm ci
COPY src/frontend/frontendservice/ .
RUN npm run build

FROM nginx:alpine
COPY --from=builder /app/dist /usr/share/nginx/html
COPY ./src/frontend/frontendservice/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 3000

CMD ["nginx", "-g", "daemon off;"]