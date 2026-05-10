# StellaFrutta Backoffice - Brief per prototipo Figma

## Obiettivo

Progettare il prototipo del backoffice StellaFrutta come interfaccia operativa del Core applicativo.
Il backoffice deve permettere agli utenti aziendali di gestire:

- prodotti
- fornitori
- lotti
- magazzino e movimenti
- controlli qualita
- ordini
- tracciabilita e audit

Il FE deve riflettere il fatto che il Core e la single source of truth per prodotti, lotti, stock, qualita e ordini.

## Utenti principali

- `Admin`: visione completa, configurazioni, utenti, supervisione
- `Commerciale`: prodotti, clienti, ordini B2B, catalogo e condizioni commerciali
- `Magazzino`: giacenze, movimenti, prelievi, preparazione e spedizione ordini
- `Qualita`: controlli su lotti, non conformita, certificazioni
- `Produzione`: visibilita operativa su lotti e avanzamento

## Obiettivo UX

Il backoffice non deve sembrare un e-commerce travestito.
Deve comunicare:

- controllo operativo
- leggibilita dei dati
- stato dei processi
- tracciabilita
- velocita nelle azioni frequenti

Quindi il prototipo dovrebbe privilegiare:

- dashboard dense ma ordinate
- tabelle con filtri forti
- schede dettaglio con timeline e pannelli laterali
- stati molto visibili
- CTA operative chiare

## Architettura informativa

Sidebar principale:

1. Dashboard
2. Prodotti
3. Fornitori
4. Lotti
5. Magazzino
6. Qualita
7. Ordini
8. Clienti
9. Audit
10. Impostazioni

Top bar:

- ricerca globale
- selettore ruolo o sede se serviranno in futuro
- notifiche
- stato sincronizzazioni o job
- profilo utente

## Schermate da prototipare per la V1

### 1. Dashboard operativa

Scopo:
offrire una vista immediata sullo stato dell'operativita.

Blocchi consigliati:

- KPI cards:
  - ordini in attesa pagamento
  - ordini da preparare
  - ordini spediti oggi
  - lotti in controllo qualita
  - prodotti sotto soglia stock
- tabella "Attivita urgenti"
- grafico movimenti stock ultimi 7 giorni
- pannello "Ultimi eventi"
- alert di non conformita o blocchi

### 2. Lista prodotti

Scopo:
gestione anagrafiche prodotto e dati commerciali.

Elementi UI:

- tabella con colonne:
  - codice
  - nome
  - categoria
  - unita di misura
  - disponibilita aggregata
  - stato
  - ultimo aggiornamento
- filtri:
  - categoria
  - attivo/non attivo
  - disponibilita
- azioni:
  - nuovo prodotto
  - esporta
  - apri dettaglio

### 3. Dettaglio prodotto

Struttura consigliata:

- header con nome, codice, stato, CTA principali
- tab con:
  - Overview
  - Lotti
  - Stock
  - Prezzi/Listini
  - Audit

Contenuti overview:

- dati base prodotto
- disponibilita totale
- ultimo lotto ricevuto
- certificazioni o attributi chiave
- mini timeline eventi recenti

### 4. Lista lotti

Scopo:
gestione della tracciabilita operativa.

Colonne consigliate:

- codice lotto
- prodotto
- fornitore
- data ricezione o creazione
- quantita residua
- stato lotto
- esito qualita
- flag BIO

Filtri chiave:

- prodotto
- fornitore
- stato
- esito qualita
- BIO/non BIO

### 5. Dettaglio lotto

Questa e una schermata centrale del backoffice.

Layout consigliato:

- colonna sinistra:
  - dati principali lotto
  - prodotto associato
  - fornitore
  - date chiave
  - quantita iniziale e residua
- colonna destra:
  - stato corrente
  - badge qualita
  - azioni rapide

Sezioni:

- timeline tracciabilita
- movimenti di magazzino
- controlli qualita
- documenti e certificazioni
- ordini collegati o prenotazioni stock

CTA principali:

- apri controllo qualita
- registra movimento
- blocca lotto
- sblocca lotto

### 6. Magazzino - vista giacenze

Scopo:
consentire lettura rapida delle disponibilita.

Elementi:

- tabella per prodotto e lotto
- filtri per disponibilita, stato, categoria
- evidenza soglia minima
- confronto:
  - disponibile
  - riservato
  - impegnato

Widget laterali:

- top prodotti in uscita
- articoli sotto soglia

### 7. Magazzino - movimenti

Scopo:
registrare e consultare carichi, scarichi, rettifiche, prelievi, spedizioni.

Elementi:

- feed tabellare cronologico
- filtri per tipo movimento, prodotto, lotto, utente, data
- drawer o modal per nuovo movimento
- badge colore per tipo movimento

### 8. Qualita - dashboard

Scopo:
monitorare controlli aperti e non conformita.

Blocchi:

- controlli aperti
- controlli in scadenza
- lotti bloccati
- non conformita aperte
- lista controlli recenti

### 9. Qualita - dettaglio controllo

Elementi:

- header con lotto, prodotto, stato controllo
- checklist esiti
- note operatore
- allegati
- CTA:
  - approva
  - respingi
  - richiedi approfondimento
  - chiudi con motivazione

### 10. Ordini - lista operativa

Scopo:
gestire ordini con forte orientamento allo stato.

Colonne:

- numero ordine
- cliente
- tipo cliente B2B o B2C
- data
- totale
- stato
- pagamento
- preparazione
- spedizione

Filtri:

- stato ordine
- data
- cliente
- tipo canale

### 11. Dettaglio ordine

Layout:

- header con numero ordine, stato, CTA
- blocco cliente e indirizzo
- tabella righe ordine
- riepilogo importi
- stato pagamento
- stato spedizione
- timeline eventi

Azioni:

- conferma preparazione
- registra spedizione
- annulla ordine
- visualizza riserve stock

## Flussi prioritari da collegare in Figma

### Flusso 1 - Creazione prodotto

1. Dashboard
2. Lista prodotti
3. Nuovo prodotto
4. Salvataggio
5. Dettaglio prodotto

### Flusso 2 - Ricezione lotto

1. Lista lotti
2. Nuovo lotto o ricezione merce
3. Compilazione dati lotto
4. Conferma
5. Dettaglio lotto
6. Apertura eventuale controllo qualita

### Flusso 3 - Gestione controllo qualita

1. Dashboard qualita
2. Lista controlli
3. Dettaglio controllo
4. Approvazione o blocco
5. Ritorno al dettaglio lotto con stato aggiornato

### Flusso 4 - Preparazione ordine

1. Lista ordini filtrata su `pagato`
2. Dettaglio ordine
3. Vista riserve stock
4. Conferma preparazione
5. Registra spedizione

## Componenti design system da preparare subito

- sidebar
- topbar
- cards KPI
- data table
- filter bar
- status badge
- stepper stati ordine
- timeline eventi
- tabs
- drawer
- modal conferma
- pannello dettaglio laterale
- toast esito azioni

## Stati da rappresentare molto bene

### Stato ordine

- creato
- in_attesa_pagamento
- pagato
- in_preparazione
- spedito
- chiuso
- annullato
- fallito

### Stato lotto

- ricevuto
- in_controllo
- disponibile
- bloccato
- esaurito

### Stato qualita

- aperto
- in_verifica
- approvato
- respinto
- chiuso

## Direzione visiva consigliata

Per il backoffice eviterei un look troppo consumer.

Direzione suggerita:

- sfondo chiaro caldo o neutro
- pannelli con contrasto medio
- verde salvia o oliva per stati positivi
- arancio ambra per warning
- rosso mattone per blocchi o non conformita
- tipografia molto leggibile
- icone sottili ma presenti
- tabelle pulite, con righe ariose

Mood:
solido, agricolo-industriale, affidabile, tracciabile.

## Sequenza pratica per costruire il prototipo in Figma

1. Imposta una pagina `Backoffice / Foundations` con colori, tipografia, badge e componenti base.
2. Crea una pagina `Backoffice / Core Flows`.
3. Disegna prima i frame desktop di:
   - Dashboard
   - Lista prodotti
   - Dettaglio lotto
   - Lista ordini
   - Dettaglio ordine
4. Collega i flow principali.
5. Solo dopo aggiungi le viste qualita e magazzino avanzate.

## Priorita consigliata

Se vuoi un primo prototipo convincente senza fare troppe schermate, partirei da queste 5:

1. Dashboard operativa
2. Lista prodotti
3. Dettaglio lotto
4. Lista ordini
5. Dettaglio ordine

Queste 5 raccontano gia molto bene il prodotto e il legame col backend.
