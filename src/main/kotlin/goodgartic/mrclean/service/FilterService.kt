package goodgartic.mrclean.service

import goodgartic.mrclean.entities.Filter
import goodgartic.mrclean.repositories.FiltersRepository
import org.springframework.stereotype.Service

@Service
class FilterService(private val repository: FiltersRepository) {

    fun allFilters(): List<Filter> = repository.findAll().toList()

    fun createFilter(pattern: String, delay: Int): Filter =
        repository.save(Filter(pattern = pattern, delay = delay))

    fun matchFilter(content: String, channel: String, user: String, roles: List<String>): Filter? =
        repository.findAll().firstOrNull {
            it.pattern.toRegex().matches(content) &&
            it.channels().matches(channel) &&
            it.users().matches(user) &&
            roles.any { role -> it.roles().matches(role) }
        }
}