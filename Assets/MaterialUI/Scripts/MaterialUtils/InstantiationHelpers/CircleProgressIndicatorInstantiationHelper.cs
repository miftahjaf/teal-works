//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;

namespace MaterialUI
{
    public class CircleProgressIndicatorInstantiationHelper : InstantiationHelper
    {
        [SerializeField]
        private GameObject m_Shadow;

        [SerializeField]
        private GameObject m_BackgroundImage;

        public override void HelpInstantiate(params InstantiationOptions[] options)
        {
            if (!options.Contains(InstantiationOptions.Raised))
            {
                DestroyImmediate(m_Shadow);
                DestroyImmediate(m_BackgroundImage);
            }

            base.HelpInstantiate(options);
        }
    }
}