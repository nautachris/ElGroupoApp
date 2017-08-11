using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ElGroupo.Domain.Data.Configurations
{
    public class EntityTypeConfiguration<TEntity> : IBuildModels where TEntity : class
    {
        public ModelBuilder ModelBuilder { get; }
        public EntityTypeBuilder<TEntity> EntityTypeBuilder => ModelBuilder.Entity<TEntity>();
        protected EntityTypeConfiguration(ModelBuilder modelBuilder)
        {
            ModelBuilder = modelBuilder;
        }

        /// <summary>
        ///     Adds or updates an annotation on the entity type. If an annotation with the key specified in
        ///     <paramref name="annotation" /> already exists it's value will be updated.
        /// </summary>
        /// <param name="annotation"> The key of the annotation to be added or updated. </param>
        /// <param name="value"> The value to be stored in the annotation. </param>
        /// <returns> The same typeBuilder instance so that multiple configuration calls can be chained. </returns>
        public virtual EntityTypeBuilder<TEntity> HasAnnotation(string annotation, object value)
        {
            return ModelBuilder.Entity<TEntity>().HasAnnotation(annotation, value);
        }

        /// <summary>
        ///     Sets the base type of this entity in an inheritance hierarchy.
        /// </summary>
        /// <param name="name"> The name of the base type. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual EntityTypeBuilder<TEntity> HasBaseType(string name)
        {
            return ModelBuilder.Entity<TEntity>().HasBaseType(name);
        }

        /// <summary>
        ///     Sets the base type of this entity in an inheritance hierarchy.
        /// </summary>
        /// <param name="entityType"> The base type. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual EntityTypeBuilder<TEntity> HasBaseType(Type entityType)
        {
            return ModelBuilder.Entity<TEntity>().HasBaseType(entityType);
        }

        /// <summary>
        ///     Sets the base type of this entity in an inheritance hierarchy.
        /// </summary>
        /// <typeparam name="TBaseType"> The base type. </typeparam>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual EntityTypeBuilder<TEntity> HasBaseType<TBaseType>()
        {
            return ModelBuilder.Entity<TEntity>().HasBaseType(typeof(TBaseType));
        }

        /// <summary>
        ///     Sets the properties that make up the primary key for this entity type.
        /// </summary>
        /// <param name="keyExpression">
        ///     <para>
        ///         A lambda expression representing the primary key property(s) (<c>blog =&gt; blog.Url</c>).
        ///     </para>
        ///     <para>
        ///         If the primary key is made up of multiple properties then specify an anonymous type including the
        ///         properties (<c>post =&gt; new { post.Title, post.BlogId }</c>).
        ///     </para>
        /// </param>
        /// <returns> An object that can be used to configure the primary key. </returns>
        public virtual KeyBuilder HasKey(Expression<Func<TEntity, object>> keyExpression)
        {
            return ModelBuilder.Entity<TEntity>().HasKey(keyExpression);

        }

        /// <summary>
        ///     Creates a new unique constraint for this entity type if one does not already exist over the specified
        ///     properties.
        /// </summary>
        /// <param name="keyExpression">
        ///     <para>
        ///         A lambda expression representing the unique constraint property(s) (<c>blog =&gt; blog.Url</c>).
        ///     </para>
        ///     <para>
        ///         If the unique constraint is made up of multiple properties then specify an anonymous type including
        ///         the properties (<c>post =&gt; new { post.Title, post.BlogId }</c>).
        ///     </para>
        /// </param>
        /// <returns> An object that can be used to configure the unique constraint. </returns>
        public virtual KeyBuilder HasAlternateKey(Expression<Func<TEntity, object>> keyExpression)
        {
            return ModelBuilder.Entity<TEntity>().HasAlternateKey(keyExpression);
        }

        /// <summary>
        ///     Returns an object that can be used to configure a property of the entity type.
        ///     If the specified property is not already part of the model, it will be added.
        /// </summary>
        /// <param name="propertyExpression">
        ///     A lambda expression representing the property to be configured (
        ///     <c>blog =&gt; blog.Url</c>).
        /// </param>
        /// <returns> An object that can be used to configure the property. </returns>
        public virtual PropertyBuilder<TProperty> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            return ModelBuilder.Entity<TEntity>().Property(propertyExpression);

        }

        /// <summary>
        ///     Excludes the given property from the entity type. This method is typically used to remove properties
        ///     from the entity type that were added by convention.
        /// </summary>
        /// <param name="propertyExpression">
        ///     A lambda expression representing the property to be ignored
        ///     (<c>blog =&gt; blog.Url</c>).
        /// </param>
        public virtual EntityTypeBuilder<TEntity> Ignore(Expression<Func<TEntity, object>> propertyExpression)
        {
            return ModelBuilder.Entity<TEntity>().Ignore(propertyExpression);
        }

        /// <summary>
        ///     Excludes the given property from the entity type. This method is typically used to remove properties
        ///     from the entity type that were added by convention.
        /// </summary>
        /// <param name="propertyName"> The name of then property to be removed from the entity type. </param>
        public virtual EntityTypeBuilder<TEntity> Ignore(string propertyName)
        {
            return ModelBuilder.Entity<TEntity>().Ignore(propertyName);
        }

        /// <summary>
        ///     Configures an index on the specified properties. If there is an existing index on the given
        ///     set of properties, then the existing index will be returned for configuration.
        /// </summary>
        /// <param name="indexExpression">
        ///     <para>
        ///         A lambda expression representing the property(s) to be included in the index
        ///         (<c>blog =&gt; blog.Url</c>).
        ///     </para>
        ///     <para>
        ///         If the index is made up of multiple properties then specify an anonymous type including the
        ///         properties (<c>post =&gt; new { post.Title, post.BlogId }</c>).
        ///     </para>
        /// </param>
        /// <returns> An object that can be used to configure the index. </returns>
        public virtual IndexBuilder HasIndex(Expression<Func<TEntity, object>> indexExpression)
        {
            return ModelBuilder.Entity<TEntity>().HasIndex(indexExpression);
        }

        /// <summary>
        ///     <para>
        ///         Configures a relationship where this entity type has a reference that points
        ///         to a single instance of the other type in the relationship.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see cref="M:Microsoft.EntityFrameworkCore.Metadata.Builders.ReferenceNavigationBuilder`2.WithMany(System.Linq.Expressions.Expression{System.Func{`1,System.Collections.Generic.IEnumerable{`0}}})" />
        ///         or
        ///         <see cref="M:Microsoft.EntityFrameworkCore.Metadata.Builders.ReferenceNavigationBuilder`2.WithOne(System.Linq.Expressions.Expression{System.Func{`1,`0}})" />
        ///         to fully configure the relationship. Calling just this method without the chained call will not
        ///         produce a valid relationship.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRelatedEntity"> The entity type that this relationship targets. </typeparam>
        /// <param name="navigationExpression">
        ///     A lambda expression representing the reference navigation property on this entity type that represents
        ///     the relationship (<c>post =&gt; post.Blog</c>). If no property is specified, the relationship will be
        ///     configured without a navigation property on this end.
        /// </param>
        /// <returns> An object that can be used to configure the relationship. </returns>
        public virtual ReferenceNavigationBuilder<TEntity, TRelatedEntity> HasOne<TRelatedEntity>(Expression<Func<TEntity, TRelatedEntity>> navigationExpression = null) where TRelatedEntity : class
        {
            return ModelBuilder.Entity<TEntity>().HasOne(navigationExpression);
        }

        /// <summary>
        ///     <para>
        ///         Configures a relationship where this entity type has a collection that contains
        ///         instances of the other type in the relationship.
        ///     </para>
        ///     <para>
        ///         After calling this method, you should chain a call to
        ///         <see cref="M:Microsoft.EntityFrameworkCore.Metadata.Builders.CollectionNavigationBuilder`2.WithOne(System.Linq.Expressions.Expression{System.Func{`1,`0}})" />
        ///         to fully configure the relationship. Calling just this method without the chained call will not
        ///         produce a valid relationship.
        ///     </para>
        /// </summary>
        /// <typeparam name="TRelatedEntity"> The entity type that this relationship targets. </typeparam>
        /// <param name="navigationExpression">
        ///     A lambda expression representing the collection navigation property on this entity type that represents
        ///     the relationship (<c>blog =&gt; blog.Posts</c>). If no property is specified, the relationship will be
        ///     configured without a navigation property on this end.
        /// </param>
        /// <returns> An object that can be used to configure the relationship. </returns>
        public virtual CollectionNavigationBuilder<TEntity, TRelatedEntity> HasMany<TRelatedEntity>(Expression<Func<TEntity, IEnumerable<TRelatedEntity>>> navigationExpression = null) where TRelatedEntity : class
        {
            return ModelBuilder.Entity<TEntity>().HasMany(navigationExpression);
        }

        /// <summary>
        ///     Configures the <see cref="T:Microsoft.EntityFrameworkCore.Metadata.ChangeTrackingStrategy" /> to be used for this entity type.
        ///     This strategy indicates how the context detects changes to properties for an instance of the entity type.
        /// </summary>
        /// <param name="changeTrackingStrategy"> The change tracking strategy to be used. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual EntityTypeBuilder<TEntity> HasChangeTrackingStrategy(ChangeTrackingStrategy changeTrackingStrategy)
        {
            return ModelBuilder.Entity<TEntity>().HasChangeTrackingStrategy(changeTrackingStrategy);
        }


        /// <summary>
        ///     Configures the table that the entity maps to when targeting a relational database.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="name"> The name of the table. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public EntityTypeBuilder ToTable(string name)
        {
            return ModelBuilder.Entity<TEntity>().ToTable(name);
        }


        /// <summary>
        ///     Configures the table that the entity maps to when targeting a relational database.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="name"> The name of the table. </param>
        /// <param name="schema"> The schema of the table. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public EntityTypeBuilder ToTable(string name, string schema)
        {
            return ModelBuilder.Entity<TEntity>().ToTable(name, schema);
        }


        /// <summary>
        ///     Configures the discriminator column used to identify which entity type each row in a table represents
        ///     when an inheritance hierarchy is mapped to a single table in a relational database.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <returns> A builder that allows the discriminator column to be configured. </returns>
        public DiscriminatorBuilder HasDiscriminator()
        {
            return ModelBuilder.Entity<TEntity>().HasDiscriminator();
        }

        /// <summary>
        ///     Configures the discriminator column used to identify which entity type each row in a table represents
        ///     when an inheritance hierarchy is mapped to a single table in a relational database.
        /// </summary>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="name"> The name of the discriminator column. </param>
        /// <param name="discriminatorType"> The type of values stored in the discriminator column. </param>
        /// <returns> A builder that allows the discriminator column to be configured. </returns>
        public DiscriminatorBuilder HasDiscriminator(string name, Type discriminatorType)
        {
            return ModelBuilder.Entity<TEntity>().HasDiscriminator(name, discriminatorType);
        }

        /// <summary>
        ///     Configures the discriminator column used to identify which entity type each row in a table represents
        ///     when an inheritance hierarchy is mapped to a single table in a relational database.
        /// </summary>
        /// <typeparam name="TDiscriminator"> The type of values stored in the discriminator column. </typeparam>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="name"> The name of the discriminator column. </param>
        /// <returns> A builder that allows the discriminator column to be configured. </returns>
        public DiscriminatorBuilder<TDiscriminator> HasDiscriminator<TDiscriminator>(string name)
        {

            return ModelBuilder.Entity<TEntity>().HasDiscriminator<TDiscriminator>(name);
        }

        /// <summary>
        ///     Configures the discriminator column used to identify which entity type each row in a table represents
        ///     when an inheritance hierarchy is mapped to a single table in a relational database.
        /// </summary>
        /// <typeparam name="TEntity"> The entity type being configured. </typeparam>
        /// <typeparam name="TDiscriminator"> The type of values stored in the discriminator column. </typeparam>
        /// <param name="entityTypeBuilder"> The builder for the entity type being configured. </param>
        /// <param name="propertyExpression">
        ///     A lambda expression representing the property to be used as the discriminator (
        ///     <c>blog =&gt; blog.Discriminator</c>).
        /// </param>
        /// <returns> A builder that allows the discriminator column to be configured. </returns>
        public DiscriminatorBuilder<TDiscriminator> HasDiscriminator<TDiscriminator>(Expression<Func<TEntity, TDiscriminator>> propertyExpression)
        {
            return ModelBuilder.Entity<TEntity>().HasDiscriminator<TEntity, TDiscriminator>(propertyExpression);
        }
    }
}
