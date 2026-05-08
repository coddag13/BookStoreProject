import './App.css'
import { useState } from 'react'

const userAccounts = [
  { id: '1', name: 'Danilo', email: 'danilo@email.com' },
  { id: '2', name: 'Ana', email: 'ana@email.com' },
  { id: '3', name: 'Marko', email: 'marko@email.com' },
]

function App() {
  const [formData, setFormData] = useState({
    bookTitle: '',
    author: '',
    quantity: '',
    account: '',
  })

  const [errorMessage, setErrorMessage] = useState('')
  const [successMessage, setSuccessMessage] = useState('')

  const handleChange = (e) => {
    const { name, value } = e.target

    setErrorMessage('')
    setSuccessMessage('')

    setFormData({
      ...formData,
      [name]: value,
    })
  }

  const handleSubmit = async (e) => {
    e.preventDefault()

    if (
      !formData.bookTitle.trim() ||
      !formData.author.trim() ||
      !formData.quantity ||
      !formData.account
    ) {
      setSuccessMessage('')
      setErrorMessage('Molimo popunite sva polja forme.')
      return
    }

    if (Number(formData.quantity) <= 0) {
      setSuccessMessage('')
      setErrorMessage('Količina mora biti veća od nule.')
      return
    }

    try {
      const response = await fetch('http://localhost:8975/purchase', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          bookTitle: formData.bookTitle,
          author: formData.author,
          quantity: Number(formData.quantity),
          accountId: formData.account,
        }),
      })

      const result = await response.json()

      if (!response.ok) {
        setSuccessMessage('')
        setErrorMessage(result.message || result.Message || 'Došlo je do greške.')
        return
      }

      setErrorMessage('')
      setSuccessMessage(
        result.message || result.Message || 'Kupovina je uspješno evidentirana.'
      )

      setFormData({
        bookTitle: '',
        author: '',
        quantity: '',
        account: '',
      })
    } catch (error) {
      setSuccessMessage('')
      setErrorMessage('Backend trenutno nije dostupan.')
    }
  }

  const isFormValid =
    formData.bookTitle.trim() &&
    formData.author.trim() &&
    formData.quantity &&
    Number(formData.quantity) > 0 &&
    formData.account

  const showQuantityError =
    formData.quantity !== '' && Number(formData.quantity) <= 0

  return (
    <main className="page">
      <section className="purchase-card">
        <p className="eyebrow">BookStore</p>
        <h1>Forma za kupovinu knjige</h1>
        <p className="intro">
          Popunite podatke u nastavku kako biste izvršili kupovinu knjige.
        </p>

        {errorMessage && <p className="message error-message">{errorMessage}</p>}
        {successMessage && (
          <p className="message success-message">{successMessage}</p>
        )}

        <form className="purchase-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="bookTitle">Naziv knjige</label>
            <input
              id="bookTitle"
              name="bookTitle"
              type="text"
              placeholder="Unesi naziv knjige"
              value={formData.bookTitle}
              onChange={handleChange}
            />
          </div>

          <div className="form-group">
            <label htmlFor="author">Autor</label>
            <input
              id="author"
              name="author"
              type="text"
              placeholder="Unesi ime autora"
              value={formData.author}
              onChange={handleChange}
            />
          </div>

          <div className="form-group">
            <label htmlFor="quantity">Količina</label>
            <input
              id="quantity"
              name="quantity"
              type="number"
              min="1"
              placeholder="Unesi količinu"
              value={formData.quantity}
              onChange={handleChange}
              className={showQuantityError ? 'input-error' : ''}
            />
            {showQuantityError && (
              <p className="field-error">Količina mora biti veća od 0.</p>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="account">Korisnički nalog</label>
            <select
              id="account"
              name="account"
              value={formData.account}
              onChange={handleChange}
            >
              <option value="" disabled>
                Izaberi nalog
              </option>
              {userAccounts.map((account) => (
                <option key={account.id} value={account.id}>
                  {account.name} - {account.email}
                </option>
              ))}
            </select>
          </div>

          <button
            type="submit"
            className="submit-button"
            disabled={!isFormValid}
          >
            Potvrdi kupovinu
          </button>
        </form>

        <section className="preview-card">
          <h2>Pregled unosa</h2>
          <div className="preview-grid">
            <div className="preview-item">
              <span>Naziv knjige</span>
              <strong>{formData.bookTitle || 'Nije uneseno'}</strong>
            </div>

            <div className="preview-item">
              <span>Autor</span>
              <strong>{formData.author || 'Nije uneseno'}</strong>
            </div>

            <div className="preview-item">
              <span>Količina</span>
              <strong>{formData.quantity || 'Nije uneseno'}</strong>
            </div>

            <div className="preview-item">
              <span>Korisnički nalog</span>
              <strong>
                {formData.account
                  ? userAccounts.find((account) => account.id === formData.account)
                      ?.name || 'Nepoznat nalog'
                  : 'Nije izabrano'}
              </strong>
            </div>
          </div>
        </section>
      </section>
    </main>
  )
}

export default App
