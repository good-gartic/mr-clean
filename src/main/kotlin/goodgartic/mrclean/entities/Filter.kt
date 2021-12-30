package goodgartic.mrclean.entities

import org.hibernate.Hibernate
import java.util.*
import javax.persistence.Entity
import javax.persistence.Id
import javax.persistence.Table

@Entity
@Table(name = "filters")
data class Filter(
    @Id
    val id: UUID = UUID.randomUUID(),

    /**
     * A regular expression pattern, that is applied
     */
    val pattern: String,

    /**
     * Limit this filter to a specific channel(s), if empty, message in all channels are filtered
     */
    val channels: String = "",

    /**
     * Limit this filter to a specific user(s), if empty, messages from all users are filtered
     */
    val users: String = "",

    /**
     * Limit this filter to a specific role(s), if empty, messages from all roles are filtered
     */
    val roles: String = "",

    /**
     * Delay in seconds, before the filtered message is deleted
     */
    val delay: Int = 0,

    /**
     * IDs of the channel, that the message should be reposted to, if empty, the message is just deleted
     */
    val repostChannels: String = "",
) {
    override fun equals(other: Any?): Boolean {
        if (this === other) return true
        if (other == null || Hibernate.getClass(this) != Hibernate.getClass(other)) return false
        if (other !is Filter) return false

        return id == other.id
    }

    override fun hashCode(): Int = javaClass.hashCode()

    override fun toString(): String = this::class.simpleName + "(id = $id)"
}