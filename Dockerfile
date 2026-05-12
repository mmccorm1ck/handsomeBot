FROM node:latest

LABEL org.opencontainers.image.source=https://github.com/mmccorm1ck/handsomeBot

WORKDIR /app

COPY server/package*.json ./

RUN npm install

COPY ./server ./

ENV PORT=3000

EXPOSE 3000

CMD ["npx", "ts-node", "src/index.ts"]
