import React from 'react';
import clsx from 'clsx';
import styles from './styles.module.css';

import NetLogo from '@site/static/img/net_core_logo.svg';
import HCLogo from '@site/static/img/hotchocolate_logo.svg';
import ReactLogo from '@site/static/img/react_logo.svg';
import RelayLogo from '@site/static/img/relay_logo.svg';

const FeatureList = [
  {
    title: 'Design the Future',
    Image: () => (
      <div className={styles.featureImage}>
        <NetLogo role="img" />
        <HCLogo role="img" />
      </div>
    ),
    description: <></>,
  },
];

function Feature({Image, title, description}) {
  return (
    <div className={clsx('col col--12')}>
      <div className="text--center">
        <Image />
      </div>
      <div className="text--center padding-horiz--md">
        <h3>{title}</h3>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures() {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          <div className={clsx('col col--offset-1 col--10')}>
            <div className="container">
              <div className="row">
                {FeatureList.map((props, idx) => (
                  <Feature key={idx} {...props} />
                ))}
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
