FROM node:20-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npx vite build


FROM caddy:latest

COPY --from=builder /app/dist /srv