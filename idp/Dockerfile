FROM unicon/shibboleth-idp:latest

MAINTAINER Chris Ryu

ENV JAVA_HOME "/opt/jre-home"

COPY shibboleth-idp/ /opt/shibboleth-idp/
COPY shib-jetty-base/ /opt/shib-jetty-base/

# RUN  yum update -y \
#     && yum install -y vim \
#     && yum clean all