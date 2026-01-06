#!/usr/bin/env python3
"""
ICA Recipe Scraper - LIVE med Database Mapping
Kräver: pip install requests beautifulsoup4 lxml

Hämtar LIVE från ICA och visar hur data mappas till databasen.
"""
import re
import sys
import json
import uuid
import requests
from bs4 import BeautifulSoup

MIDDAG_URL = "https://www.ica.se/recept/middag/"


def get_first_recipe_url(html):
    """Hitta första receptet från middagssidan"""
    soup = BeautifulSoup(html, 'lxml')
    
    for element in soup.find_all(attrs={'data-recipe-link-url': True}):
        url = element.get('data-recipe-link-url')
        title = element.get('data-recipe-link-name') or element.get('data-recipe-name')
        
        if url and title:
            return url, title
    
    for link in soup.find_all('a', href=True):
        href = link['href']
        
        if re.match(r'^/recept/[\w-]+-\d{5,}/?$', href):
            full_url = f"https://www.ica.se{href}"
            title = link.get_text(strip=True)
            
            if title:
                return full_url, title
    
    return None, None


def scrape_recipe(html, url):
    """Scrapa all info från receptsidan"""
    soup = BeautifulSoup(html, 'lxml')
    
    title_tag = soup.find('h1')
    title = title_tag.get_text(strip=True) if title_tag else "Okänd"
    
    description = None
    desc_tag = soup.find('meta', attrs={'name': 'description'})
    if desc_tag and desc_tag.get('content'):
        description = desc_tag['content']
    
    image_url = None
    img_tag = soup.find('img', src=re.compile(r'imagevault', re.I))
    if img_tag and img_tag.get('src'):
        image_url = img_tag['src']
    
    rating = None
    rating_text = soup.find(string=re.compile(r'Betyg\s+[\d.]+\s+av', re.I))
    if rating_text:
        match = re.search(r'([\d.]+)\s+av\s+5', str(rating_text))
        if match:
            rating = float(match.group(1))
    
    rating_count = None
    votes_text = soup.find(string=re.compile(r'\d+\s*personer har röstat', re.I))
    if votes_text:
        match = re.search(r'(\d+)\s*personer', str(votes_text))
        if match:
            rating_count = int(match.group(1))
    
    comment_count = None
    comments_text = soup.find(string=re.compile(r'\d+\s*kommentarer?', re.I))
    if comments_text:
        match = re.search(r'(\d+)\s*kommentar', str(comments_text))
        if match:
            comment_count = int(match.group(1))
    
    prep_time = None
    time_text = soup.find(string=re.compile(r'Under \d+|Över \d+', re.I))
    if time_text:
        if 'Under 30' in str(time_text):
            prep_time = 30
        elif 'Under 45' in str(time_text):
            prep_time = 45
        elif 'Under 60' in str(time_text):
            prep_time = 60
        elif 'Över 60' in str(time_text):
            prep_time = 90
    
    difficulty = None
    for diff in ['Enkel', 'Medel', 'Avancerad']:
        if soup.find(string=re.compile(f'^{diff}$', re.I)):
            difficulty = diff
            break
    
    servings = None
    servings_text = soup.find(string=re.compile(r'\d+\s*portion', re.I))
    if servings_text:
        match = re.search(r'(\d+)\s*portion', str(servings_text))
        if match:
            servings = int(match.group(1))
    
    # Ingredienser
    ingredients = []
    for card in soup.find_all('div', class_=re.compile('ingredients-list-group__card')):
        text = card.get_text(strip=True)
        if text and len(text) > 2:
            clean_text = re.sub(r'^\d+\s*', '', text)
            if clean_text and text not in ingredients:
                ingredients.append(text)
    
    # Instruktioner
    instructions = []
    for card in soup.find_all('div', class_=re.compile('cooking-steps-card')):
        label = card.find('label', class_=re.compile('ids-checkbox-label'))
        if label:
            text = label.get_text(strip=True)
            text = re.sub(r'Klicka i här.*$', '', text, flags=re.I).strip()
            
            if text and len(text) > 10 and text not in instructions:
                instructions.append(text)
    
    is_climate_smart = bool(soup.find(string=re.compile('klimatsmart', re.I)))
    
    return {
        'url': url,
        'title': title,
        'description': description,
        'image_url': image_url,
        'rating': rating,
        'rating_count': rating_count,
        'comment_count': comment_count,
        'prep_time_minutes': prep_time,
        'difficulty': difficulty,
        'servings': servings,
        'is_climate_smart': is_climate_smart,
        'ingredients': ingredients,
        'instructions': instructions,
    }


def prepare_for_database(recipe_data):
    """Förbered scrapad data för databas-insert"""
    
    # Fix mellanslag i ingredienser: "4port" → "4 port"
    ingredients_fixed = []
    for ing in recipe_data['ingredients']:
        # Lägg till mellanslag efter siffror och mellan tal och text
        fixed = re.sub(r'(\d+)([a-zA-ZåäöÅÄÖ])', r'\1 \2', ing)
        # Fix decimal: "2 1/2dl" → "2 1/2 dl"
        fixed = re.sub(r'(\d+/\d+)([a-zA-ZåäöÅÄÖ])', r'\1 \2', fixed)
        ingredients_fixed.append(fixed)
    
    # Generera tags från scrapad data
    tags = []
    if recipe_data.get('difficulty'):
        tags.append(recipe_data['difficulty'].lower())
    if recipe_data.get('prep_time_minutes', 999) <= 30:
        tags.append('snabbt')
    if recipe_data.get('is_climate_smart'):
        tags.append('klimatsmart')
    tags.append('vardagsmat')
    
    # Cook time formatering
    cook_time = None
    if recipe_data.get('prep_time_minutes'):
        mins = recipe_data['prep_time_minutes']
        if mins <= 30:
            cook_time = 'Under 30 min'
        elif mins <= 45:
            cook_time = 'Under 45 min'
        elif mins <= 60:
            cook_time = 'Under 60 min'
        else:
            cook_time = 'Över 60 min'
    
    return {
        'id': str(uuid.uuid4()),
        'title': recipe_data['title'],
        'image_url': recipe_data.get('image_url'),
        'cook_time': cook_time,
        'servings': recipe_data.get('servings'),
        'calories': None,
        'protein': None,
        'carbs': None,
        'fat': None,
        'ingredients': ingredients_fixed,
        'instructions': recipe_data['instructions'],
        'tags': tags,
        'created_by': None,
        'is_public': True,
    }


def print_database_mapping(scraped, db_ready):
    """Visa hur scrapad data mappas till databas-format"""
    
    print("\n" + "=" * 80)
    print("📊 DATABASE MAPPING")
    print("=" * 80)
    
    print("\n📥 SCRAPAD DATA från ICA:")
    print("-" * 80)
    print(f"URL: {scraped['url']}")
    print(f"Titel: {scraped['title']}")
    print(f"Beskrivning: {scraped.get('description', 'N/A')}")
    print(f"Betyg: {scraped.get('rating')} ({scraped.get('rating_count')} röster)")
    print(f"Tillagningstid: {scraped.get('prep_time_minutes')} min")
    print(f"Svårighetsgrad: {scraped.get('difficulty')}")
    print(f"Portioner: {scraped.get('servings')}")
    print(f"Klimatsmart: {scraped.get('is_climate_smart')}")
    print(f"Ingredienser: {len(scraped['ingredients'])} st")
    print(f"Instruktioner: {len(scraped['instructions'])} steg")
    
    print("\n" + "=" * 80)
    print("💾 DATABAS-FORMAT (recipes table)")
    print("=" * 80)
    
    # SQL INSERT statement
    print("\nDB_PRINT(\"\"\"")
    print("INSERT INTO recipes (")
    print("    id, title, image_url, cook_time, servings,")
    print("    calories, protein, carbs, fat,")
    print("    ingredients, instructions, tags,")
    print("    created_by, is_public, created_at")
    print(") VALUES (")
    print(f"    '{db_ready['id']}',  -- UUID genererat")
    print(f"    '{db_ready['title']}',")
    print(f"    '{db_ready['image_url']}',")
    print(f"    '{db_ready['cook_time']}',")
    print(f"    {db_ready['servings']},")
    print(f"    NULL,  -- calories (ICA har ej)")
    print(f"    NULL,  -- protein (ICA har ej)")
    print(f"    NULL,  -- carbs (ICA har ej)")
    print(f"    NULL,  -- fat (ICA har ej)")
    
    # JSONB ingredients
    print(f"    '{json.dumps(db_ready['ingredients'], ensure_ascii=False)}'::JSONB,")
    
    # JSONB instructions
    print(f"    '{json.dumps(db_ready['instructions'], ensure_ascii=False)}'::JSONB,")
    
    # Array tags
    tags_str = "{" + ",".join(db_ready['tags']) + "}"
    print(f"    ARRAY{tags_str}::TEXT[],")
    
    print(f"    NULL,  -- created_by (system-scrapad)")
    print(f"    TRUE,  -- is_public")
    print(f"    NOW()  -- created_at")
    print(");")
    print("\"\"\")\n")
    
    # Detaljerad uppdelning
    print("\n" + "=" * 80)
    print("🔍 DETALJERAD MAPPNING")
    print("=" * 80)
    
    print("\n1️⃣  INGREDIENTS (JSONB) - MED FIX:")
    print("-" * 80)
    print("FÖRE fix:")
    for i, ing in enumerate(scraped['ingredients'][:3], 1):
        print(f"  {i}. '{ing}'")
    print("  ...")
    
    print("\nEFTER fix (mellanslag tillagt):")
    for i, ing in enumerate(db_ready['ingredients'][:3], 1):
        print(f"  {i}. '{ing}'")
    print("  ...")
    
    print(f"\nJSON format:")
    print(json.dumps(db_ready['ingredients'], indent=2, ensure_ascii=False))
    
    print("\n2️⃣  INSTRUCTIONS (JSONB):")
    print("-" * 80)
    for i, step in enumerate(db_ready['instructions'], 1):
        print(f"  {i}. {step}")
    
    print("\n3️⃣  TAGS (TEXT[]) - GENERERADE:")
    print("-" * 80)
    print(f"  Från difficulty='{scraped.get('difficulty')}' → '{scraped.get('difficulty', '').lower()}'")
    print(f"  Från prep_time={scraped.get('prep_time_minutes')} min → 'snabbt' (om ≤30)")
    print(f"  Från is_climate_smart={scraped.get('is_climate_smart')} → 'klimatsmart' (om True)")
    print(f"  Default → 'vardagsmat'")
    print(f"\n  Resultat: {db_ready['tags']}")
    
    print("\n4️⃣  COOK_TIME (VARCHAR):")
    print("-" * 80)
    print(f"  Från prep_time_minutes={scraped.get('prep_time_minutes')}")
    print(f"  Till cook_time='{db_ready['cook_time']}'")
    
    print("\n5️⃣  NULL-värden (data som ICA inte har):")
    print("-" * 80)
    print(f"  calories: {db_ready['calories']}")
    print(f"  protein: {db_ready['protein']}")
    print(f"  carbs: {db_ready['carbs']}")
    print(f"  fat: {db_ready['fat']}")
    print(f"  created_by: {db_ready['created_by']} (system-scrapad)")
    
    print("\n" + "=" * 80)


def main():
    print("ICA Recipe Scraper - LIVE med Database Mapping")
    print("=" * 80)
    
    print(f"\n1️⃣  Hämtar {MIDDAG_URL}")
    
    try:
        response = requests.get(
            MIDDAG_URL,
            headers={"User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"},
            timeout=30
        )
        response.raise_for_status()
        print(f"   ✓ Status: {response.status_code}")
    except Exception as e:
        print(f"   ✗ Fel: {e}\n")
        return
    
    print("\n2️⃣  Letar efter första receptet...")
    
    recipe_url, recipe_title = get_first_recipe_url(response.text)
    if not recipe_url:
        print("   ✗ Hittade inga recept!")
        return
    
    print(f"   ✓ {recipe_title}")
    
    print(f"\n3️⃣  Hämtar receptsidan...")
    
    try:
        response = requests.get(
            recipe_url,
            headers={"User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"},
            timeout=30
        )
        response.raise_for_status()
        print(f"   ✓ Status: {response.status_code}")
    except Exception as e:
        print(f"   ✗ Fel: {e}\n")
        return
    
    print("\n4️⃣  Scrapar data...")
    scraped = scrape_recipe(response.text, recipe_url)
    print("   ✓ Klar!")
    
    print("\n5️⃣  Förbereder för databas...")
    db_ready = prepare_for_database(scraped)
    print("   ✓ Klar!")
    
    # Visa mappningen
    print_database_mapping(scraped, db_ready)


if __name__ == "__main__":
    main()