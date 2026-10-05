export type UserSummary = { id: string; username: string; displayName: string; role: string; faculty: string; avatarUrl: string }
export type UserProfile = UserSummary & { email: string; bio: string; createdAt: string; postCount: number; communityCount: number }
export type Comment = { id: string; content: string; author: UserSummary; createdAt: string }
export type Post = { id: string; content: string; imageUrl: string; author: UserSummary; communityId?: string; communityName?: string; createdAt: string; commentCount: number; reactions: Record<string, number>; comments: Comment[] }
export type Community = { id: string; name: string; slug: string; description: string; owner: UserSummary; memberCount: number; isMember: boolean; createdAt: string }
export type AuthResponse = { token: string; user: UserSummary }
export type Dashboard = { profile: UserProfile; posts: number; comments: number; reactionsReceived: number; communities: number; recentPosts: Post[] }
export type SearchResponse = { users: UserSummary[]; posts: Post[]; communities: Community[] }

const API_URL = import.meta.env.VITE_API_URL || (import.meta.env.DEV ? 'http://localhost:5050' : '')

export async function api<T>(path: string, options: RequestInit = {}, token?: string): Promise<T> {
  const response = await fetch(`${API_URL}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers,
    },
  })
  if (!response.ok) {
    const payload = await response.json().catch(() => null) as { title?: string; detail?: string; errors?: Record<string, string[]> } | null
    const validation = payload?.errors ? Object.values(payload.errors).flat().join(' ') : ''
    throw new Error(validation || payload?.detail || payload?.title || `Error ${response.status}`)
  }
  if (response.status === 204) return undefined as T
  return response.json() as Promise<T>
}
