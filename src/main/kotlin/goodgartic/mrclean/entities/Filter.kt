package goodgartic.mrclean.entities

import org.hibernate.Hibernate
import javax.persistence.*

@Entity
@Table(name = "filters")
data class Filter(
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    val id: Int = 0,

    /**
     * A regular expression pattern, that is applied
     */
    val pattern: String,

    /**
     * Delay in seconds, before the filtered message is deleted
     */
    val delay: Int = 0,

    /**
     * Limit this filter to a specific channel(s), if empty, message in all channels are filtered
     */
    private val channels: String = "",

    /**
     * Limit this filter to a specific user(s), if empty, messages from all users are filtered
     */
    private val users: String = "",

    /**
     * Limit this filter to a specific role(s), if empty, messages from all roles are filtered
     */
    private val roles: String = "",

    /**
     * IDs of the channel, that the message should be reposted to, if empty, the message is just deleted
     */
    private val repostChannels: String = "",
) {
    fun channels(): FilterSpecification = channels.toFilterSpecification()
    fun users(): FilterSpecification = users.toFilterSpecification()
    fun roles(): FilterSpecification = roles.toFilterSpecification()
    fun repostChannels(): List<String> = repostChannels.split(",")

    override fun equals(other: Any?): Boolean {
        if (this === other) return true
        if (other == null || Hibernate.getClass(this) != Hibernate.getClass(other)) return false
        if (other !is Filter) return false

        return id == other.id
    }

    override fun hashCode(): Int = javaClass.hashCode()

    override fun toString(): String = this::class.simpleName + "(id = $id)"
}