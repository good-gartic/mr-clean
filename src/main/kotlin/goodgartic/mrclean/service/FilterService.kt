package goodgartic.mrclean.service

import goodgartic.mrclean.entities.Filter
import goodgartic.mrclean.repositories.FiltersRepository
import net.dv8tion.jda.api.entities.Message
import net.dv8tion.jda.api.entities.TextChannel
import org.springframework.stereotype.Service

@Service
class FilterService(private val repository: FiltersRepository) {

    fun allFilters(): List<Filter> = repository.findAll().toList()

    fun createFilter(pattern: String, delay: Long): Filter =
        repository.save(Filter(pattern = pattern, delay = delay))

    fun matchFilter(content: String, channel: String, user: String, roles: List<String>): Filter? =
        repository.findAll().firstOrNull {
            it.pattern.toRegex().matches(content) &&
            it.channels().matches(channel) &&
            it.users().matches(user) &&
            roles.any { role -> it.roles().matches(role) }
        }

    fun applyFilter(message: Message, filter: Filter) {
        // Repost only if the repost channel was configured
        if (filter.repostChannel.isNotBlank()) {
            val channel = message.guild.getTextChannelById(filter.repostChannel)
                ?: throw IllegalStateException("Cannot find the configured reposting channel [id]")

            repostMessage(message, channel)
        }

        message.delete()
            .reason("Matched filter [id = ${filter.id}, pattern = ${filter.pattern}]")
            .queue()
    }

    private fun repostMessage(message: Message, channel: TextChannel) {
        TODO("Implement reposting messages")
    }
}