package goodgartic.mrclean.service

import goodgartic.mrclean.configuration.Constants
import goodgartic.mrclean.entities.Filter
import goodgartic.mrclean.repositories.FiltersRepository
import net.dv8tion.jda.api.EmbedBuilder
import net.dv8tion.jda.api.entities.Message
import net.dv8tion.jda.api.entities.TextChannel
import org.springframework.data.repository.findByIdOrNull
import org.springframework.stereotype.Service
import java.io.File

@Service
class FilterService(private val repository: FiltersRepository) {

    fun allFilters(): List<Filter> = repository.findAll().toList()

    fun findFilter(id: Long): Filter? = repository.findByIdOrNull(id)

    fun createFilter(pattern: String, delay: Long, channel: String = ""): Filter =
        repository.save(Filter(pattern = pattern, delay = delay, repostChannel = channel))

    fun updateFilter(filter: Filter): Filter =
        repository.save(filter)

    fun deleteFilter(filter: Filter) =
        repository.delete(filter)

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
        val embed = EmbedBuilder()
            .setAuthor(message.author.name, null, message.author.effectiveAvatarUrl)
            .setDescription(message.contentRaw)
            .setColor(Constants.Colors.primary)
            .setFooter("Originally posted in #${message.channel.name}")
            .setTimestamp(message.timeEdited ?: message.timeCreated)

        val action = channel.sendMessageEmbeds(message.embeds + embed.build())
        val files = message.attachments.map {
            it.downloadToFile(File.createTempFile("temp_", it.fileName))
                .join()
                .also { file -> action.addFile(file) }
        }

        action.complete()

        files.forEach { it.delete() }
    }
}