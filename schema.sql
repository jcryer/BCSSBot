--
-- PostgreSQL database dump
--

-- Dumped from database version 12.4 (Ubuntu 12.4-1.pgdg16.04+1)
-- Dumped by pg_dump version 12.4

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: memberships; Type: TABLE; Schema: public; Owner: bcss
--

CREATE TABLE public.memberships (
    user_hash bigint,
    permission_id bigint
);


ALTER TABLE public.memberships OWNER TO bcss;

--
-- Name: permissions; Type: TABLE; Schema: public; Owner: bcss
--

CREATE TABLE public.permissions (
    permission_id bigint NOT NULL,
    permission_string text,
    permission_name text
);


ALTER TABLE public.permissions OWNER TO bcss;

--
-- Name: permissions_permission_id_seq; Type: SEQUENCE; Schema: public; Owner: bcss
--

ALTER TABLE public.permissions ALTER COLUMN permission_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.permissions_permission_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: users; Type: TABLE; Schema: public; Owner: bcss
--

CREATE TABLE public.users (
    user_hash integer NOT NULL,
    discord_id numeric,
    email text
);


ALTER TABLE public.users OWNER TO bcss;

--
-- Data for Name: memberships; Type: TABLE DATA; Schema: public; Owner: bcss
--

COPY public.memberships (user_hash, permission_id) FROM stdin;
\.


--
-- Data for Name: permissions; Type: TABLE DATA; Schema: public; Owner: bcss
--

COPY public.permissions (permission_id, permission_string, permission_name) FROM stdin;
9	{"items":[{"discordid":523964143053832213,"type":1},{"discordid":520715386488881180,"type":0}]}	test-1
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: bcss
--

COPY public.users (user_hash, discord_id, email) FROM stdin;
-1745389512	\N	jcryer1234@gmail.com
1280882509	306895088095723529	
\.


--
-- Name: permissions_permission_id_seq; Type: SEQUENCE SET; Schema: public; Owner: bcss
--

SELECT pg_catalog.setval('public.permissions_permission_id_seq', 29, true);


--
-- Name: permissions permissions_permission_id_key; Type: CONSTRAINT; Schema: public; Owner: bcss
--

ALTER TABLE ONLY public.permissions
    ADD CONSTRAINT permissions_permission_id_key UNIQUE (permission_id);


--
-- Name: permissions permissions_pkey; Type: CONSTRAINT; Schema: public; Owner: bcss
--

ALTER TABLE ONLY public.permissions
    ADD CONSTRAINT permissions_pkey PRIMARY KEY (permission_id);


--
-- Name: users users_email_key; Type: CONSTRAINT; Schema: public; Owner: bcss
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_key UNIQUE (email);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: bcss
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_hash);


--
-- Name: users users_user_hash_key; Type: CONSTRAINT; Schema: public; Owner: bcss
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_user_hash_key UNIQUE (user_hash);


--
-- Name: memberships memberships_permission_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: bcss
--

ALTER TABLE ONLY public.memberships
    ADD CONSTRAINT memberships_permission_id_fkey FOREIGN KEY (permission_id) REFERENCES public.permissions(permission_id);


--
-- Name: memberships memberships_user_hash_fkey; Type: FK CONSTRAINT; Schema: public; Owner: bcss
--

ALTER TABLE ONLY public.memberships
    ADD CONSTRAINT memberships_user_hash_fkey FOREIGN KEY (user_hash) REFERENCES public.users(user_hash);


--
-- PostgreSQL database dump complete
--

