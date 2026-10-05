export type UserSummary = { id: string; username: string; displayName: string; role: string; faculty: string; avatarUrl: string }
export type UserProfile = UserSummary & { email: string; bio: string; createdAt: string; postCount: number; communityCount: number }
export type Comment = { id: string; content: string; author: UserSummary; createdAt: string }
export type Post = { id: string; content: string; imageUrl: string; author: UserSummary; communityId?: string; communityName?: string; createdAt: string; commentCount: number; reactions: Record<string, number>; comments: Comment[] }
export type Community = { id: string; name: string; slug: string; description: string; owner: UserSummary; memberCount: number; isMember: boolean; createdAt: string }
export type AuthResponse = { token: string; user: UserSummary }
export type Dashboard = { profile: UserProfile; posts: number; comments: number; reactionsReceived: number; communities: number; recentPosts: Post[] }
export type SearchResponse = { users: UserSummary[]; posts: Post[]; communities: Community[] }

const API_URL = import.meta.env.VITE_API_URL || (import.meta.env.DEV ? 'http://localhost:5050' : '')
const STATIC_DEMO = import.meta.env.VITE_STATIC_DEMO === 'true'
const STORAGE_KEY = 'campusconecta.pages.data.v1'
type DemoUser = Omit<UserProfile, 'postCount' | 'communityCount'> & { password: string }
type DemoPost = { id: string; content: string; imageUrl: string; authorId: string; communityId?: string; createdAt: string; comments: Array<{ id: string; content: string; authorId: string; createdAt: string }>; reactions: Record<string, string> }
type DemoCommunity = { id: string; name: string; slug: string; description: string; ownerId: string; memberIds: string[]; createdAt: string }
type DemoData = { users: DemoUser[]; posts: DemoPost[]; communities: DemoCommunity[] }
const uid = () => globalThis.crypto?.randomUUID?.() || `${Date.now()}-${Math.random().toString(16).slice(2)}`
const now = () => new Date().toISOString()

function initialData(): DemoData {
  const createdAt = now()
  const gabriela: DemoUser = { id: 'gabriela', email: 'gabriela@campus.test', username: 'gabriela', displayName: 'Gabriela Cohaila', role: 'Student', faculty: 'Ingeniería de Sistemas', avatarUrl: '', bio: 'Estudiante interesada en arquitectura web y comunidades de aprendizaje.', createdAt, password: 'Demo123!' }
  const victoria: DemoUser = { id: 'victoria', email: 'victoria@campus.test', username: 'victoria', displayName: 'Victoria Lavarello', role: 'Student', faculty: 'Ingeniería de Sistemas', avatarUrl: '', bio: 'Comparto iniciativas académicas y experiencias de calidad de software.', createdAt, password: 'Demo123!' }
  const docente: DemoUser = { id: 'docente', email: 'docente@campus.test', username: 'docente', displayName: 'Dr. Luis Ramos', role: 'Teacher', faculty: 'Facultad de Ingeniería', avatarUrl: '', bio: 'Docente y acompañante de proyectos universitarios.', createdAt, password: 'Demo123!' }
  const comunidad: DemoCommunity = { id: 'innovacion', name: 'Innovación y proyectos', slug: 'innovacion-proyectos', description: 'Espacio para ideas, retos y prototipos universitarios.', ownerId: docente.id, memberIds: [gabriela.id, victoria.id, docente.id], createdAt }
  return { users: [gabriela, victoria, docente], communities: [comunidad], posts: [
    { id: 'post-1', authorId: docente.id, communityId: comunidad.id, content: 'Bienvenidas y bienvenidos a CampusConecta. Compartan aquí sus avances y oportunidades de colaboración.', imageUrl: '', createdAt, comments: [{ id: 'comment-1', authorId: gabriela.id, content: '¡Lista para compartir nuestro proyecto!', createdAt }], reactions: { victoria: 'Like' } },
    { id: 'post-2', authorId: gabriela.id, content: 'Estamos organizando una reunión para revisar buenas prácticas de APIs REST. ¿Quién se suma?', imageUrl: '', createdAt, comments: [], reactions: { docente: 'Celebrate' } },
  ] }
}
function readData(): DemoData { try { const saved = localStorage.getItem(STORAGE_KEY); if (saved) return JSON.parse(saved) as DemoData } catch { /* Se recrea el conjunto de demostración. */ } const value = initialData(); writeData(value); return value }
function writeData(value: DemoData) { localStorage.setItem(STORAGE_KEY, JSON.stringify(value)) }
function fail(message: string): never { throw new Error(message) }
function userSummary(user: DemoUser): UserSummary { const { id, username, displayName, role, faculty, avatarUrl } = user; return { id, username, displayName, role, faculty, avatarUrl } }
function current(data: DemoData, token?: string) { const id = token?.replace('local-', ''); const user = data.users.find(item => item.id === id); return user || fail('Sesión no válida. Ingresa nuevamente.') }
function profile(data: DemoData, user: DemoUser): UserProfile { return { ...userSummary(user), email: user.email, bio: user.bio, createdAt: user.createdAt, postCount: data.posts.filter(post => post.authorId === user.id).length, communityCount: data.communities.filter(group => group.memberIds.includes(user.id)).length } }
function postResponse(data: DemoData, post: DemoPost): Post {
  const author = data.users.find(user => user.id === post.authorId) || fail('Autor no encontrado.')
  const community = post.communityId ? data.communities.find(group => group.id === post.communityId) : undefined
  const reactions = Object.values(post.reactions).reduce<Record<string, number>>((result, type) => ({ ...result, [type]: (result[type] || 0) + 1 }), {})
  const comments = post.comments.map(comment => { const commentAuthor = data.users.find(user => user.id === comment.authorId) || fail('Autor no encontrado.'); return { id: comment.id, content: comment.content, author: userSummary(commentAuthor), createdAt: comment.createdAt } })
  return { id: post.id, content: post.content, imageUrl: post.imageUrl, author: userSummary(author), communityId: post.communityId, communityName: community?.name, createdAt: post.createdAt, commentCount: comments.length, reactions, comments }
}
function communityResponse(data: DemoData, group: DemoCommunity, userId: string): Community { const owner = data.users.find(user => user.id === group.ownerId) || fail('Responsable no encontrado.'); return { id: group.id, name: group.name, slug: group.slug, description: group.description, owner: userSummary(owner), memberCount: group.memberIds.length, isMember: group.memberIds.includes(userId), createdAt: group.createdAt } }

async function staticApi<T>(path: string, options: RequestInit, token?: string): Promise<T> {
  const data = readData(); const method = (options.method || 'GET').toUpperCase(); const body = options.body ? JSON.parse(String(options.body)) as Record<string, string> : {}
  if (path === '/api/auth/login' && method === 'POST') { const user = data.users.find(item => item.email.toLowerCase() === body.email?.trim().toLowerCase() && item.password === body.password); if (!user) return fail('El correo o la contraseña no son correctos.'); return { token: `local-${user.id}`, user: userSummary(user) } as T }
  if (path === '/api/auth/register' && method === 'POST') { if (!body.email || !body.username || !body.displayName || !body.password || body.password.length < 8) return fail('Completa los datos requeridos; la contraseña debe tener al menos 8 caracteres.'); if (data.users.some(item => item.email.toLowerCase() === body.email.toLowerCase() || item.username.toLowerCase() === body.username.toLowerCase())) return fail('El correo o nombre de usuario ya está registrado.'); const user: DemoUser = { id: uid(), email: body.email.trim().toLowerCase(), username: body.username.trim().toLowerCase(), displayName: body.displayName.trim(), password: body.password, role: body.role || 'Student', faculty: body.faculty?.trim() || 'Comunidad universitaria', avatarUrl: '', bio: '', createdAt: now() }; data.users.push(user); writeData(data); return { token: `local-${user.id}`, user: userSummary(user) } as T }
  const me = current(data, token)
  if (path === '/api/users/me' && method === 'GET') return profile(data, me) as T
  if (path === '/api/users/me' && method === 'PUT') { me.displayName = body.displayName?.trim() || me.displayName; me.faculty = body.faculty?.trim() || ''; me.bio = body.bio?.trim() || ''; me.avatarUrl = body.avatarUrl?.trim() || ''; writeData(data); return profile(data, me) as T }
  if (path === '/api/posts' && method === 'GET') return data.posts.slice().sort((a, b) => b.createdAt.localeCompare(a.createdAt)).map(post => postResponse(data, post)) as T
  if (path === '/api/posts' && method === 'POST') { if (!body.content?.trim()) return fail('La publicación no puede estar vacía.'); const post: DemoPost = { id: uid(), authorId: me.id, content: body.content.trim(), imageUrl: body.imageUrl?.trim() || '', createdAt: now(), comments: [], reactions: {} }; data.posts.push(post); writeData(data); return postResponse(data, post) as T }
  const match = path.match(/^\/api\/posts\/([^/]+)\/(reaction|comments)$/)
  if (match) { const post = data.posts.find(item => item.id === match[1]) || fail('Publicación no encontrada.'); if (match[2] === 'reaction' && method === 'PUT') { post.reactions[me.id] = body.type || 'Like'; writeData(data); return postResponse(data, post) as T } if (match[2] === 'comments' && method === 'POST') { if (!body.content?.trim()) return fail('El comentario no puede estar vacío.'); post.comments.push({ id: uid(), authorId: me.id, content: body.content.trim(), createdAt: now() }); writeData(data); return {} as T } }
  if (path === '/api/communities' && method === 'GET') return data.communities.map(group => communityResponse(data, group, me.id)) as T
  if (path === '/api/communities' && method === 'POST') { if (!body.name?.trim() || !body.description?.trim()) return fail('Ingresa nombre y descripción de la comunidad.'); const group: DemoCommunity = { id: uid(), name: body.name.trim(), slug: body.name.trim().toLowerCase().replace(/[^a-z0-9]+/g, '-'), description: body.description.trim(), ownerId: me.id, memberIds: [me.id], createdAt: now() }; data.communities.push(group); writeData(data); return communityResponse(data, group, me.id) as T }
  const communityMatch = path.match(/^\/api\/communities\/([^/]+)\/(join|leave)$/)
  if (communityMatch) { const group = data.communities.find(item => item.id === communityMatch[1]) || fail('Comunidad no encontrada.'); group.memberIds = communityMatch[2] === 'join' ? Array.from(new Set([...group.memberIds, me.id])) : group.memberIds.filter(id => id !== me.id); writeData(data); return {} as T }
  if (path === '/api/dashboard' && method === 'GET') { const mine = data.posts.filter(post => post.authorId === me.id); const reactions = mine.reduce((sum, post) => sum + Object.keys(post.reactions).length, 0); const comments = data.posts.reduce((sum, post) => sum + post.comments.filter(comment => comment.authorId === me.id).length, 0); return { profile: profile(data, me), posts: mine.length, comments, reactionsReceived: reactions, communities: data.communities.filter(group => group.memberIds.includes(me.id)).length, recentPosts: mine.slice().sort((a, b) => b.createdAt.localeCompare(a.createdAt)).slice(0, 5).map(post => postResponse(data, post)) } as T }
  if (path.startsWith('/api/search') && method === 'GET') { const term = new URL(path, 'https://campus.local').searchParams.get('q')?.toLowerCase().trim() || ''; return { users: data.users.filter(user => `${user.displayName} ${user.username} ${user.faculty}`.toLowerCase().includes(term)).map(userSummary), posts: data.posts.filter(post => post.content.toLowerCase().includes(term)).map(post => postResponse(data, post)), communities: data.communities.filter(group => `${group.name} ${group.description}`.toLowerCase().includes(term)).map(group => communityResponse(data, group, me.id)) } as T }
  return fail('La operación solicitada no está disponible en la versión estática.')
}

export async function api<T>(path: string, options: RequestInit = {}, token?: string): Promise<T> {
  if (STATIC_DEMO) return staticApi<T>(path, options, token)
  const response = await fetch(`${API_URL}${path}`, { ...options, headers: { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}), ...options.headers } })
  if (!response.ok) { const payload = await response.json().catch(() => null) as { title?: string; detail?: string; errors?: Record<string, string[]> } | null; const validation = payload?.errors ? Object.values(payload.errors).flat().join(' ') : ''; throw new Error(validation || payload?.detail || payload?.title || `Error ${response.status}`) }
  if (response.status === 204) return undefined as T
  return response.json() as Promise<T>
}
