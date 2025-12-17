FROM node:18-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npx run build


FROM caddy:latest

COPY --from=builder /app/dist /srv