using FluentValidation;

namespace Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Product.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Product.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Product.Price)
            .GreaterThan(0);

        RuleFor(x => x.Product.Stock)
            .GreaterThanOrEqualTo(0);
    }
}
