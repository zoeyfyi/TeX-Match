'use strict';

import { traverse } from 'svgo/lib/xast.js';

const name = 'ApplyTranslations';
const type = 'full';
const active = true;
const description = 'applys translations to x and y';

/**
 * Applys translations to elements with an x and y attribute
 *
 *
 * @param {Object} document document element
 * @param {Object} options plugin params
 *
 * @author zoeyfyi <me@zoey.fyi>
 */
const fn = function (document) {
    traverse(document, node => {
        if (node.type !== 'element')
            return;

        if (node.attributes.x === null || node.attributes.y === null || node.attributes.transform === undefined)
            return;

        const coords = node.attributes.transform.match(/translate\((-?[0-9]*\.[0-9]*) (-?[0-9]*\.[0-9]*)\)/)

        if (coords.length !== 3)
            return;

        const xTrans = parseFloat(coords[1]);
        const yTrans = parseFloat(coords[2]);

        const x = parseFloat(node.attributes.x);
        const y = parseFloat(node.attributes.y);

        if (xTrans === NaN || yTrans === NaN || x === NaN || y === NaN)
            return;

        node.attributes.x = x + xTrans;
        node.attributes.y = y + yTrans;
        node.removeAttr("transform");
    });

    return document;
};

export default { name, type, active, description, fn };