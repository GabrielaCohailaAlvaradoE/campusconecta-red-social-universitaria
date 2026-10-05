import { useCallback, useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import { api, type AuthResponse, type Community, type Dashboard, type Post, type SearchResponse, type UserProfile, type UserSummary } from './api'
import './App.css'

type View = 'feed' | 'communities' | 'dashboard' | 'profile' | 'search'
const roleLabels: Record<string, string> = { Student: 'Estudiante', Teacher: 'Docente', Staff: 'Personal' }
const initials = (name: string) => name.split(' ').slice(0, 2).map(x => x[0]).join('').toUpperCase()
const when = (value: string) => new Intl.DateTimeFormat('es-PE', { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value))

function Avatar({ user, large = false }: { user: UserSummary; large?: boolean }) {
  return user.avatarUrl
    ? <img className={`avatar ${large ? 'large' : ''}`} src={user.avatarUrl} alt={`Foto de ${user.displayName}`} />
    : <span className={`avatar avatar-fallback ${large ? 'large' : ''}`} aria-hidden="true">{initials(user.displayName)}</span>
}

function AuthScreen({ onAuth }: { onAuth: (auth: AuthResponse) => void }) {
  const [mode, setMode] = useState<'login' | 'register'>('login')
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const [login, setLogin] = useState({ email: 'gabriela@campus.test', password: 'Demo123!' })
  const [register, setRegister] = useState({ email: '', username: '', displayName: '', password: '', role: 'Student', faculty: '' })

  const submit = async (event: FormEvent) => {
    event.preventDefault(); setBusy(true); setError('')
    try {
      const path = mode === 'login' ? '/api/auth/login' : '/api/auth/register'
      const auth = await api<AuthResponse>(path, { method: 'POST', body: JSON.stringify(mode === 'login' ? login : register) })
      onAuth(auth)
    } catch (reason) { setError(reason instanceof Error ? reason.message : 'No se pudo completar el acceso.') }
    finally { setBusy(false) }
  }

  return <main className="auth-page">
    <section className="auth-story">
      <div className="brand"><span className="brand-mark">CC</span><span>CampusConecta</span></div>
      <div className="story-copy">
        <span className="eyebrow">TU CAMPUS EN UN SOLO LUGAR</span>
        <h1>Ideas, personas y oportunidades que sí se encuentran.</h1>
        <p>Una red segura para aprender, compartir proyectos y construir comunidad universitaria.</p>
        <div className="story-stats"><div><strong>3</strong><span>roles conectados</span></div><div><strong>1</strong><span>comunidad universitaria</span></div><div><strong>24/7</strong><span>colaboración</span></div></div>
      </div>
      <p className="story-foot">Diseñada para estudiantes, docentes y personal universitario.</p>
    </section>
    <section className="auth-panel">
      <div className="auth-card">
        <div className="mobile-brand"><span className="brand-mark">CC</span>CampusConecta</div>
        <span className="eyebrow">BIENVENIDA</span>
        <h2>{mode === 'login' ? 'Vuelve a tu comunidad' : 'Crea tu cuenta universitaria'}</h2>
        <p>{mode === 'login' ? 'Ingresa con tus credenciales para ver las novedades del campus.' : 'Completa tus datos y empieza a participar.'}</p>
        <div className="auth-tabs"><button className={mode === 'login' ? 'active' : ''} onClick={() => setMode('login')}>Ingresar</button><button className={mode === 'register' ? 'active' : ''} onClick={() => setMode('register')}>Registrarme</button></div>
        <form onSubmit={submit}>
          {mode === 'register' && <><label>Nombre completo<input value={register.displayName} onChange={e => setRegister({ ...register, displayName: e.target.value })} required minLength={2} /></label><label>Nombre de usuario<input value={register.username} onChange={e => setRegister({ ...register, username: e.target.value })} required pattern="[a-zA-Z0-9._]{3,30}" /></label></>}
          <label>Correo institucional<input type="email" value={mode === 'login' ? login.email : register.email} onChange={e => mode === 'login' ? setLogin({ ...login, email: e.target.value }) : setRegister({ ...register, email: e.target.value })} required /></label>
          <label>Contraseña<input type="password" value={mode === 'login' ? login.password : register.password} onChange={e => mode === 'login' ? setLogin({ ...login, password: e.target.value }) : setRegister({ ...register, password: e.target.value })} required minLength={8} /></label>
          {mode === 'register' && <div className="form-row"><label>Rol<select value={register.role} onChange={e => setRegister({ ...register, role: e.target.value })}><option value="Student">Estudiante</option><option value="Teacher">Docente</option><option value="Staff">Personal</option></select></label><label>Facultad<input value={register.faculty} onChange={e => setRegister({ ...register, faculty: e.target.value })} /></label></div>}
          {error && <p className="form-error" role="alert">{error}</p>}
          <button className="primary full" disabled={busy}>{busy ? 'Procesando…' : mode === 'login' ? 'Ingresar a CampusConecta' : 'Crear mi cuenta'}</button>
        </form>
        {mode === 'login' && <p className="demo-note"><strong>Cuenta de demostración</strong><br />gabriela@campus.test · Demo123!</p>}
      </div>
    </section>
  </main>
}

function PostCard({ post, token, onChanged }: { post: Post; token: string; onChanged: () => void }) {
  const [comment, setComment] = useState('')
  const [busy, setBusy] = useState(false)
  const react = async (type: string) => { setBusy(true); await api(`/api/posts/${post.id}/reaction`, { method: 'PUT', body: JSON.stringify({ type }) }, token); setBusy(false); onChanged() }
  const addComment = async (event: FormEvent) => { event.preventDefault(); if (!comment.trim()) return; setBusy(true); await api(`/api/posts/${post.id}/comments`, { method: 'POST', body: JSON.stringify({ content: comment }) }, token); setComment(''); setBusy(false); onChanged() }
  return <article className="post-card">
    <header><Avatar user={post.author} /><div><strong>{post.author.displayName}</strong><span>@{post.author.username} · {when(post.createdAt)}</span></div>{post.communityName && <span className="community-pill">{post.communityName}</span>}</header>
    <p className="post-copy">{post.content}</p>
    {post.imageUrl && <img className="post-image" src={post.imageUrl} alt="Contenido de la publicación" />}
    <div className="reaction-bar"><button disabled={busy} onClick={() => react('Like')}>♥ Me gusta <b>{post.reactions.Like || 0}</b></button><button disabled={busy} onClick={() => react('Celebrate')}>✦ Celebrar <b>{post.reactions.Celebrate || 0}</b></button><button disabled={busy} onClick={() => react('Insightful')}>● Útil <b>{post.reactions.Insightful || 0}</b></button><span>{post.commentCount} comentarios</span></div>
    {post.comments.length > 0 && <div className="comments">{post.comments.slice(-2).map(item => <div key={item.id}><Avatar user={item.author} /><p><strong>{item.author.displayName}</strong>{item.content}</p></div>)}</div>}
    <form className="comment-form" onSubmit={addComment}><input aria-label="Escribir comentario" placeholder="Aporta a la conversación…" value={comment} onChange={e => setComment(e.target.value)} maxLength={500} /><button disabled={busy || !comment.trim()}>Enviar</button></form>
  </article>
}

function App() {
  const [auth, setAuth] = useState<AuthResponse | null>(() => { try { return JSON.parse(localStorage.getItem('campus.auth') || 'null') as AuthResponse | null } catch { return null } })
  const [view, setView] = useState<View>('feed')
  const [profile, setProfile] = useState<UserProfile | null>(null)
  const [posts, setPosts] = useState<Post[]>([])
  const [communities, setCommunities] = useState<Community[]>([])
  const [dashboard, setDashboard] = useState<Dashboard | null>(null)
  const [search, setSearch] = useState<SearchResponse | null>(null)
  const [query, setQuery] = useState('')
  const [postText, setPostText] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const token = auth?.token || ''

  const loadCore = useCallback(async () => {
    if (!token) return
    setLoading(true); setError('')
    try {
      const [me, feed, groups] = await Promise.all([api<UserProfile>('/api/users/me', {}, token), api<Post[]>('/api/posts', {}, token), api<Community[]>('/api/communities', {}, token)])
      setProfile(me); setPosts(feed); setCommunities(groups)
    } catch (reason) { setError(reason instanceof Error ? reason.message : 'No se pudo cargar el contenido.') }
    finally { setLoading(false) }
  }, [token])

  useEffect(() => {
    if (!auth) return
    const timer = window.setTimeout(() => void loadCore(), 0)
    return () => window.clearTimeout(timer)
  }, [auth, loadCore])
  const handleAuth = (value: AuthResponse) => { localStorage.setItem('campus.auth', JSON.stringify(value)); setAuth(value) }
  const logout = () => { localStorage.removeItem('campus.auth'); setAuth(null); setProfile(null) }
  const selectView = async (next: View) => { setView(next); setError(''); if (next === 'dashboard') setDashboard(await api<Dashboard>('/api/dashboard', {}, token)) }
  const submitPost = async (event: FormEvent) => { event.preventDefault(); if (!postText.trim()) return; await api('/api/posts', { method: 'POST', body: JSON.stringify({ content: postText }) }, token); setPostText(''); await loadCore() }
  const doSearch = async (event: FormEvent) => { event.preventDefault(); if (query.trim().length < 2) return; setSearch(await api<SearchResponse>(`/api/search?q=${encodeURIComponent(query)}`, {}, token)); setView('search') }
  const joined = useMemo(() => communities.filter(x => x.isMember), [communities])

  if (!auth) return <AuthScreen onAuth={handleAuth} />
  return <div className="app-shell">
    <header className="topbar"><button className="brand brand-button" onClick={() => void selectView('feed')}><span className="brand-mark">CC</span><span>CampusConecta</span></button><form className="search-box" onSubmit={doSearch}><span>⌕</span><input value={query} onChange={e => setQuery(e.target.value)} placeholder="Buscar personas, publicaciones o comunidades" /><kbd>Enter</kbd></form><div className="top-actions"><button aria-label="Notificaciones" className="icon-button">●</button><button className="user-menu" onClick={() => void selectView('profile')}><Avatar user={auth.user} /><span><strong>{profile?.displayName || auth.user.displayName}</strong><small>{roleLabels[auth.user.role]}</small></span></button></div></header>
    <aside className="sidebar"><nav><button className={view === 'feed' ? 'active' : ''} onClick={() => void selectView('feed')}><span>⌂</span>Inicio</button><button className={view === 'communities' ? 'active' : ''} onClick={() => void selectView('communities')}><span>◎</span>Comunidades</button><button className={view === 'dashboard' ? 'active' : ''} onClick={() => void selectView('dashboard')}><span>▦</span>Mi panel</button><button className={view === 'profile' ? 'active' : ''} onClick={() => void selectView('profile')}><span>○</span>Mi perfil</button></nav><div className="side-groups"><span>MIS COMUNIDADES</span>{joined.slice(0, 4).map(group => <button key={group.id} onClick={() => void selectView('communities')}><i>{group.name[0]}</i>{group.name}</button>)}</div><button className="logout" onClick={logout}>Salir de la cuenta</button></aside>
    <main className="content">{error && <div className="error-banner">{error}</div>}{loading && <div className="loading">Actualizando tu campus…</div>}{view === 'feed' && <FeedView profile={profile} posts={posts} postText={postText} setPostText={setPostText} submitPost={submitPost} token={token} reload={() => void loadCore()} communities={communities} />}{view === 'communities' && <CommunitiesView items={communities} token={token} reload={() => void loadCore()} />}{view === 'dashboard' && dashboard && <DashboardView data={dashboard} />}{view === 'profile' && profile && <ProfileView profile={profile} token={token} onSaved={loadCore} />}{view === 'search' && search && <SearchView data={search} token={token} reload={() => void loadCore()} />}</main>
  </div>
}

function FeedView({ profile, posts, postText, setPostText, submitPost, token, reload, communities }: { profile: UserProfile | null; posts: Post[]; postText: string; setPostText: (value: string) => void; submitPost: (event: FormEvent) => void; token: string; reload: () => void; communities: Community[] }) {
  return <div className="feed-layout"><section><div className="page-heading"><div><span className="eyebrow">INICIO</span><h1>Tu comunidad hoy</h1><p>Ideas, novedades y conversaciones de todo el campus.</p></div><span className="date-chip">Actualizado ahora</span></div><form className="composer" onSubmit={submitPost}>{profile && <Avatar user={profile} />}<div><textarea value={postText} onChange={e => setPostText(e.target.value)} placeholder="¿Qué quieres compartir con tu comunidad?" maxLength={1500} /><footer><span>{postText.length}/1500</span><button className="primary" disabled={!postText.trim()}>Publicar</button></footer></div></form><div className="feed-filter"><strong>Publicaciones recientes</strong><span>{posts.length} resultados</span></div>{posts.map(post => <PostCard key={post.id} post={post} token={token} onChanged={reload} />)}</section><aside className="right-rail"><div className="rail-card"><span className="eyebrow">COMUNIDADES</span><h3>Encuentra tu espacio</h3>{communities.slice(0, 3).map(group => <div className="mini-group" key={group.id}><i>{group.name[0]}</i><div><strong>{group.name}</strong><span>{group.memberCount} miembros</span></div></div>)}</div><div className="rail-card accent"><span>Consejo del día</span><p>Comparte el contexto de tu proyecto para recibir comentarios más útiles.</p></div></aside></div>
}

function CommunitiesView({ items, token, reload }: { items: Community[]; token: string; reload: () => void }) {
  const [showCreate, setShowCreate] = useState(false); const [name, setName] = useState(''); const [description, setDescription] = useState('')
  const create = async (event: FormEvent) => { event.preventDefault(); await api('/api/communities', { method: 'POST', body: JSON.stringify({ name, description }) }, token); setName(''); setDescription(''); setShowCreate(false); reload() }
  const membership = async (item: Community) => { await api(`/api/communities/${item.id}/${item.isMember ? 'leave' : 'join'}`, { method: item.isMember ? 'DELETE' : 'POST' }, token); reload() }
  return <section><div className="page-heading"><div><span className="eyebrow">COMUNIDADES</span><h1>Espacios para colaborar</h1><p>Conecta con personas que comparten tus intereses académicos.</p></div><button className="primary" onClick={() => setShowCreate(!showCreate)}>+ Crear comunidad</button></div>{showCreate && <form className="inline-form" onSubmit={create}><label>Nombre<input value={name} onChange={e => setName(e.target.value)} required minLength={3} /></label><label>Descripción<input value={description} onChange={e => setDescription(e.target.value)} required minLength={10} /></label><button className="primary">Crear</button></form>}<div className="community-grid">{items.map(item => <article className="community-card" key={item.id}><div className="community-letter">{item.name[0]}</div><span className="member-count">{item.memberCount} miembros</span><h2>{item.name}</h2><p>{item.description}</p><small>Creada por {item.owner.displayName}</small><button className={item.isMember ? 'secondary' : 'primary'} onClick={() => void membership(item)}>{item.isMember ? 'Salir' : 'Unirme'}</button></article>)}</div></section>
}

function DashboardView({ data }: { data: Dashboard }) {
  const stats: [string, number, string][] = [['Publicaciones', data.posts, '↑'], ['Comentarios', data.comments, '↗'], ['Reacciones recibidas', data.reactionsReceived, '♥'], ['Comunidades', data.communities, '◎']]
  return <section><div className="page-heading"><div><span className="eyebrow">MI PANEL</span><h1>Tu actividad en perspectiva</h1><p>Un resumen claro de tu participación en CampusConecta.</p></div></div><div className="stats-grid">{stats.map(([label, value, icon]) => <div className="stat-card" key={label}><span>{icon}</span><strong>{value}</strong><small>{label}</small></div>)}</div><div className="panel-card"><h2>Actividad reciente</h2>{data.recentPosts.map(post => <div className="activity-row" key={post.id}><i>●</i><div><p>{post.content}</p><span>{when(post.createdAt)} · {post.commentCount} comentarios</span></div></div>)}</div></section>
}

function ProfileView({ profile, token, onSaved }: { profile: UserProfile; token: string; onSaved: () => Promise<void> }) {
  const [form, setForm] = useState({ displayName: profile.displayName, bio: profile.bio, faculty: profile.faculty, avatarUrl: profile.avatarUrl }); const [saved, setSaved] = useState(false)
  const save = async (event: FormEvent) => { event.preventDefault(); await api('/api/users/me', { method: 'PUT', body: JSON.stringify(form) }, token); await onSaved(); setSaved(true) }
  return <section><div className="profile-hero"><Avatar user={profile} large /><div><span className="eyebrow">MI PERFIL</span><h1>{profile.displayName}</h1><p>@{profile.username} · {roleLabels[profile.role]}</p></div></div><form className="profile-form panel-card" onSubmit={save}><h2>Información personal</h2><div className="form-row"><label>Nombre visible<input value={form.displayName} onChange={e => setForm({ ...form, displayName: e.target.value })} required /></label><label>Facultad o área<input value={form.faculty} onChange={e => setForm({ ...form, faculty: e.target.value })} /></label></div><label>Biografía<textarea value={form.bio} onChange={e => setForm({ ...form, bio: e.target.value })} maxLength={320} /></label><label>URL de fotografía<input type="url" value={form.avatarUrl} onChange={e => setForm({ ...form, avatarUrl: e.target.value })} /></label><div className="save-row">{saved && <span className="success">Cambios guardados</span>}<button className="primary">Guardar perfil</button></div></form></section>
}

function SearchView({ data, token, reload }: { data: SearchResponse; token: string; reload: () => void }) {
  return <section><div className="page-heading"><div><span className="eyebrow">BÚSQUEDA</span><h1>Resultados encontrados</h1><p>{data.users.length} personas, {data.posts.length} publicaciones y {data.communities.length} comunidades.</p></div></div>{data.users.length > 0 && <div className="panel-card"><h2>Personas</h2><div className="people-list">{data.users.map(user => <div key={user.id}><Avatar user={user} /><p><strong>{user.displayName}</strong><span>@{user.username} · {roleLabels[user.role]}</span></p></div>)}</div></div>}{data.communities.length > 0 && <div className="community-grid compact">{data.communities.map(group => <article className="community-card" key={group.id}><div className="community-letter">{group.name[0]}</div><h2>{group.name}</h2><p>{group.description}</p><span>{group.memberCount} miembros</span></article>)}</div>}{data.posts.map(post => <PostCard key={post.id} post={post} token={token} onChanged={reload} />)}</section>
}

export default App
