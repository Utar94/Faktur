import Vue from 'vue'
import { ValidationObserver, ValidationProvider, extend, localize } from 'vee-validate'
import { confirmed, email, max, max_value, min, min_value, required } from 'vee-validate/dist/rules'

import password from './rules/password'
import url from './rules/url'

import en from './en.json'
import fr from './fr.json'

Vue.component('validation-observer', ValidationObserver)
Vue.component('validation-provider', ValidationProvider)

extend('confirmed', confirmed)
extend('email', email)
extend('max', max)
extend('max_value', max_value)
extend('min', min)
extend('min_value', min_value)
extend('password', password)
extend('required', required)
extend('url', url)

localize({ en, fr })
